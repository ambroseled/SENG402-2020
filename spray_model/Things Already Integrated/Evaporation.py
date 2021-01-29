def calc_evaporation_model(Dia, Re, bulbTemp, time):
    # Function that returns the evaporation model of a droplet
    # Dia is the diameter of the droplet in metres
    # Re is the Reynolds number of the droplet
    # bulbTemp is the wet bulb temperature depression in degrees Celsius

    Te = (Dia ** 2) / (84.76 * bulbTemp * (1 + (0.27 * (Re ** 0.5))))

    evapModel = -3 / ((2 * Te) * (1 - (t / Te)))

    return evapModel