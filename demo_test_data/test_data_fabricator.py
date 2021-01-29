import csv

dataset_top_car_park = {
    "a": (-43.52005, 172.56692),
    "b": (-43.52057, 172.56692),
    "c": (-43.52005, 172.56775),
    "d": (-43.52057, 172.56775)
}

dataset_middle_car_park = {
    "a": (-43.5206, 172.56657),
    "b": (-43.52114, 172.56657),
    "c": (-43.5206, 172.56736),
    "d": (-43.52114, 172.56736)
}

dataset_bottom_car_park = {
    "a": (-43.52188, 172.5657),
    "b": (-43.52312, 172.5657),
    "c": (-43.52188, 172.56704),
    "d": (-43.52312, 172.56704)
}

step = 0.00001  # Approximation of 1.11m for lat/long --> https://www.usna.edu/Users/oceano/pguth/md_help/html/approx_equivalents.htm#:~:text=Approximate%20Metric%20Equivalents%20for%20Degrees&text=At%20the%20equator%20for%20longitude,0.1%C2%B0%20%3D%2011.1%20km


def make_grid(dataset):
    """
    Method to create a grid out of the four corners defined in WGS84 coordinates
    :param dataset: dictionary of values a, b, c, d where a = top left, b = bot left, c = top right, d = bot right
    :return: 2D array of tuples making up grid between corners with spacing of 1.11m
    """
    top_left_lat = dataset["a"][0]
    top_left_lng = dataset["a"][1]
    top_right_lng = dataset["c"][1]
    bot_left_lat = dataset["b"][0]

    lng_row = []
    lat_col = []
    i = top_left_lng
    while i < top_right_lng:
        lng_row.append(round(i, 5))
        i += step
    j = bot_left_lat
    while j < top_left_lat:
        lat_col.append(round(j, 5))
        j += step
    out_grid = []
    for i in lat_col:
        row = []
        for j in lng_row:
            row.append("{0}:{1}:0".format(i, j))
        out_grid.append(row)
    return out_grid


def save_to_csv(grid, filename):
    with open(filename, mode='w') as file:
        writer = csv.writer(file, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
        for row in grid:
            writer.writerow(row)


if __name__ == '__main__':
    grid = make_grid(dataset_top_car_park)
    filename = "SENG402-demo-data-top.csv"
    save_to_csv(grid, filename)
