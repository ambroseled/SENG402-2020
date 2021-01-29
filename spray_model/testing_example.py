import unittest
import numpy as np
from helicopter_spray_model import calc_drag

"""
This file shows how to get started with unit testing in Python using unittest.

The concept for this application is that we want to sanity check our functions. These can be imported from another
modules for testing, e.g. from helicopter_spray_model import get_coordinates

NOTE: each test case MUST start with "def test_" if the function name starts with anything else, it will not be run.

numpy has a testing module built in and it can be used to test for equality or almost equality
(https://numpy.org/doc/stable/reference/routines.testing.html?highlight=testing#module-numpy.testing)
"""

# NOTE: this could be imported
def area_of_a_circle(radius):
    """
    Calculates the area of a circle given its radius.
    """
    return np.pi * radius ** 2


class SampleCircleAreaTestCases(unittest.TestCase):
    def test_area_is_approximately_correct(self):
        self.assertAlmostEqual(area_of_a_circle(10), 314.2, places=1)
        # places is the number of decimal places we approximate to
        # you can alternatively provide a desired delta using delta=

    def test_area_is_zero_when_we_input_zero(self):
        self.assertEquals(area_of_a_circle(0), 0)

    def test_this_one_fails(self):
        self.assertEqual(1, 2) # should throw an error and tell us why


class SampleNumpyArraysTestCases(unittest.TestCase):
    def test_arrays_are_similar(self):
        numbers = np.ndarray((1, 2, 3, 4))
        numbers_plus_point_1 = numbers + 0.1
        np.testing.assert_array_almost_equal(numbers, numbers_plus_point_1, decimal=1)

    def test_this_one_fails(self):
        numbers = np.ndarray((1, 2, 3, 4))
        numbers_plus_point_1 = numbers + 0.1
        np.testing.assert_array_almost_equal(numbers, numbers_plus_point_1, decimal=2)


class HelicopterSprayModelTestCases(unittest.TestCase):
    def test_drag_is_zero_when_Re_is_zero(self):
        # TODO: incorrect assumption by Exe, fix or delete this
        self.assertEqual(calc_drag(0), 0)


if __name__ == '__main__':
    unittest.main()
