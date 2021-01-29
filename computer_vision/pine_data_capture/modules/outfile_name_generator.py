import os
import logging

COMMON_FILE_PREFIX = 'rgbd_pine_data_output'
FILE_EXTENSION = 'bag'
TEMPLATE = '{}/{}_{}.{}'
OUT_FILES_PATH = 'out_files'

LOG_TAG = 'OutfileNameGenerator'


class OutfileNameGenerator:
    def __init__(self):
        if not os.path.exists(OUT_FILES_PATH):
            os.makedirs(OUT_FILES_PATH)

    def get_latest_file_number(self) -> int:
        files = [f for f in os.listdir(OUT_FILES_PATH) if os.path.isfile(
            os.path.join(OUT_FILES_PATH, f))]
        return len(files)

    def get_next_filename(self) -> str:
        file_number = self.get_latest_file_number()
        next_filename = TEMPLATE.format(
            OUT_FILES_PATH, COMMON_FILE_PREFIX, file_number, FILE_EXTENSION)
        logging.info('{}: generated new file name: {}'.format(
            LOG_TAG, next_filename))
        return next_filename


if __name__ == "__main__":
    if os.path.exists(OUT_FILES_PATH):
        os.removedirs(OUT_FILES_PATH)
    filename_generator = OutfileNameGenerator()
    print(filename_generator.get_next_filename())
