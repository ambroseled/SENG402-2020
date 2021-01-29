#!/usr/bin/python3

import pyrealsense2 as rs
import numpy as np
import cv2

'''
Idea is to pass RGB frame through an object detector & segmentator,
and be able to overlay depth data on top of each detected object.

NOTE: OpenCV works with the BGR color space.
'''

X_RESOLUTION = 848
Y_RESOLUTION = 480
FRAME_RATE = 30

RGB_ORANGE = (243, 156, 18)
BGR_ORANGE = tuple(reversed(RGB_ORANGE))

FONT_FACE = 0
FONT_SCALE = 1
FONT_COLOR = BGR_ORANGE

RECTANGLE_COLOR = BGR_ORANGE


def get_depth_frame(aligned_frames: rs.composite_frame) -> np.ndarray:
    return np.asanyarray(aligned_frames.get_depth_frame().get_data())


def get_color_frame(aligned_frames: rs.composite_frame) -> np.ndarray:
    return np.asanyarray(aligned_frames.get_color_frame().get_data())


def get_depth_color_map(depth_frame: np.ndarray):
    return cv2.applyColorMap(cv2.convertScaleAbs(depth_frame, alpha=0.03), cv2.COLORMAP_JET)


def detect_faces(color_frame: np.ndarray, face_cascade: cv2.CascadeClassifier):
    """
    :return: an iterable of (x, y, w, h) tuples with bounding rectangles for the detected faces.
    """
    scaling = 1.3
    min_neighbours = 5
    gray_frame = cv2.cvtColor(color_frame, cv2.COLOR_BGR2GRAY)
    return face_cascade.detectMultiScale(gray_frame, scaling, min_neighbours)


def draw_bounding_boxes(faces, color_frame: np.ndarray, depth_frame: np.ndarray, depth_scale: float) -> None:
    """
    Draw bounding rectangles and average distance to each face, in place.
    """
    for (x, y, width, height) in faces:
        # draw box around face
        rectangle_thickness = 2
        cv2.rectangle(color_frame, (x, y), (x + width, y + height), RECTANGLE_COLOR, rectangle_thickness)

        # add the average distance to the face
        depth_region_of_interest = depth_frame[y:y + height, x:x + width]
        average_distance, _, _, _ = cv2.mean(
            depth_region_of_interest)  # mean returns mean per channel for up to 4 channels, the other 3 three are unused and zero
        cv2.putText(color_frame, f'{average_distance * depth_scale:.2f}m', (x, y), FONT_FACE, FONT_SCALE,
                    FONT_COLOR)


def should_close_window() -> bool:
    delay_in_ms = 5
    return cv2.waitKey(delay_in_ms) & 0xFF == ord('q')


if __name__ == '__main__':
    config = rs.config()
    config.enable_stream(rs.stream.color, X_RESOLUTION, Y_RESOLUTION, rs.format.bgr8,
                         FRAME_RATE)  # OpenCV uses the BGR format
    config.enable_stream(rs.stream.depth, X_RESOLUTION, Y_RESOLUTION, rs.format.z16, FRAME_RATE)

    # owns the handles to all connected RealSense devices
    pipeline = rs.pipeline()
    profile = pipeline.start(config)

    align = rs.align(rs.stream.color)

    # scaling factor to convert to metres
    depth_scale: float = profile.get_device().first_depth_sensor().get_depth_scale()

    face_cascade = cv2.CascadeClassifier('haarcascade_frontalface_default.xml')

    try:
        while True:
            frames = pipeline.wait_for_frames()
            aligned_frames = align.process(frames)

            depth_frame = get_depth_frame(aligned_frames)
            color_frame = get_color_frame(aligned_frames)

            # visualise depth frames with pretty colours
            depth_colormap = get_depth_color_map(depth_frame)

            faces = detect_faces(color_frame, face_cascade)

            draw_bounding_boxes(faces, color_frame, depth_frame, depth_scale)
            draw_bounding_boxes(faces, depth_colormap, depth_frame, depth_scale)

            # stack and display images
            images = np.hstack((color_frame, depth_colormap))
            cv2.imshow('Wilding Pines Object Detection', images)

            if should_close_window():
                break
    finally:
        pipeline.stop()
        cv2.destroyAllWindows()
