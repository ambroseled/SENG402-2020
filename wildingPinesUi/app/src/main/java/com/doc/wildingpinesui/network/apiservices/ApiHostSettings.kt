package com.doc.wildingpinesui.network.apiservices

/**
 * Used to configure whether to send requests to the NUC or to development host.
 */
object ApiHostSettings {
    private const val localhost = "10.0.2.2:5000"
    private const val nucHost = "10.42.0.1:5000"
    private const val host = nucHost
    const val apiBaseUrl = "http://$host/api/v1/"
}