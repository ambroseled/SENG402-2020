package com.doc.wildingpinesui.model

/**
 * Represents the status of the whole spraying system.
 */
enum class SystemStatus {
    OK, // the NUC is reachable and everything is ready to go
    NUC_UNREACHABLE // the tablet can not reach the NUC
}