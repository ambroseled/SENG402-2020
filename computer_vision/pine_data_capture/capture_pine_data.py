#!/usr/bin/python3

# capture_pine_data.py

import pyrealsense2 as rs
import numpy as np
import cv2
import logging
import time
import os
import sys

from modules.graceful_killer import GracefulKiller
from modules.outfile_name_generator import OutfileNameGenerator

LOG_FILE_NAME = 'wilding_pines_dev.log'

"""
| Frame rate | X Resolution | Y Resolution | Approx. size / min | Mins. of footage with 90GB of storage |
| :--------: | :----------: | :----------: | :----------------: | :----------------------------------:  |
| 6          | 640          | 480          | 312MB              | 270                                   |
| 6          | 848          | 480          | 397MB              | 226                                   |
| 30         | 640          | 480          | 1.8GB              | 50                                    |
| 30         | 848          | 480          | 2.4GB              | 37.5                                  |
"""

FRAME_RATE = 6
# FRAME_RATE = 30

# X_RESOLUTION = 640
X_RESOLUTION = 848
Y_RESOLUTION = 480


class WildingPinesDataCapturer():
    def __init__(self, logging, outfile_name_generator, realsense, cv2, graceful_killer, show_windows=False):
        self.logging = logging.getLogger()
        self.outfile_name_generator = outfile_name_generator
        self.rs = realsense
        self.cv2 = cv2
        self.graceful_killer = graceful_killer
        self._should_show_windows = show_windows

        self.out_file_name = ''
        self.rs_config = None
        self.pipeline = None
        self.profile = None
        self.align = None

    def set_up(self):
        """
        Set up:
        - Logging
        - RealSense camera

        Preconditions: None.
        """
        self.out_file_name = outfile_name_generator.get_next_filename()
        self.logging.info('Out file set to {}'.format(self.out_file_name))

        # Enable the depth and color streams.
        # The rs.format.bgr8 indicates that the colour data is three 8-bit channels.
        # Note the bgr (blue, green, red) format for use with OpenCV.
        # The rs.format.z16 indicates that the depth data is an unsigned 16-bit integer.
        self.rs_config = rs.config()
        self.rs_config.enable_stream(self.rs.stream.color, X_RESOLUTION, Y_RESOLUTION, self.rs.format.bgr8, FRAME_RATE)
        self.rs_config.enable_stream(self.rs.stream.depth, X_RESOLUTION, Y_RESOLUTION, self.rs.format.z16, FRAME_RATE)
        self.logging.info('Using {} FPS, x resolution of {}, and y resolution of {}'.format(FRAME_RATE, X_RESOLUTION, Y_RESOLUTION))
        self.logging.info('Configured RealSense camera')

    def start_capture(self):
        """
        Start recording.

        Preconditions: the client has run the set_up method.
        """
        self.rs_config.enable_record_to_file(self.out_file_name)
        self.logging.info('Enabled recording from depth camera to out file')

        self.pipeline = self.rs.pipeline()  # Create a pipeline
        self.logging.info('Set up camera pipeline')
        self.profile = self.pipeline.start(self.rs_config)  # Start streaming
        self.logging.info('Started streaming from camera')
        self.align = self.rs.align(self.rs.stream.color)  # Create the alignment object.
        self.logging.info('Created alignment object. Recording...')

    def stop_capture(self):
        """
        Stop recording.

        Preconditions: the client has run the start_capture method.
        """
        self.logging.info('Stopping wilding pines data collection program')
        # Stop the camera and close the GUI windows.
        self.pipeline.stop()
        self.logging.info('Stopped depth camera pipeline')
        # cv2.destroyAllWindows()
        # logging.info('Destroyed OpenCV windows')

    def update_cv2_windows(self):
        """
        Update the cv2 windows with new frames from the camera.
        """
        # Get frameset of color and depth and align the frames.
        frames = self.pipeline.wait_for_frames()
        aligned_frames = self.align.process(frames)

        # Get aligned frames.
        depth_image = np.asanyarray(aligned_frames.get_depth_frame().get_data())
        color_image = np.asanyarray(aligned_frames.get_color_frame().get_data())

        # Show the depth and color data to the screen.
        self.cv2.imshow('Colour ', color_image)
        self.cv2.imshow('Depth', depth_image)

        # Close the script when q is pressed. Only works when there are windows.
        if self.cv2.waitKey(1) & 0xFF == ord('q'):
            self.graceful_killer.request_program_exit('User pressed the q key')

    def should_show_windows(self) -> bool:
        """
        Whether the data capturer is supposed to show OpenCV windows.
        """
        return self._should_show_windows

if __name__ == "__main__":
    print('Running wilding pines program, now logging to {}'.format(LOG_FILE_NAME))
    logging.basicConfig(
        filename=LOG_FILE_NAME,
        level=logging.DEBUG,
        format='%(asctime)s - %(levelname)s - %(message)s'
    )
    logging.info('Started wilding pines program')
    logging.info('Process PID is {}'.format(os.getpid()))

    try:
        outfile_name_generator = OutfileNameGenerator()
        graceful_killer = GracefulKiller(logging)
        logging.debug('Instantiated graceful killer')

        logging.info('Waiting 10 secs before trying to connect to the camera')
        time.sleep(10)

        if graceful_killer.should_kill_now():
            logging.info('Stopped program')
            sys.exit()

        data_capturer = WildingPinesDataCapturer(logging, outfile_name_generator, rs, cv2, graceful_killer, show_windows=False)
        logging.debug('Instantiated data capturer')

        data_capturer.set_up()
        data_capturer.start_capture()

        while not graceful_killer.should_kill_now():
            if data_capturer.should_show_windows():
                data_capturer.update_cv2_windows()

        data_capturer.stop_capture()
        logging.info('Stopped program')
    except Exception as ex:
        logging.error('Runtime exception: {}'.format(ex))
        sys.exit(1)
