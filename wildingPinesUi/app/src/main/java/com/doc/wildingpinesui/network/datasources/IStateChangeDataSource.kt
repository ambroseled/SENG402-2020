package com.doc.wildingpinesui.network.datasources

import androidx.lifecycle.LiveData
import com.doc.wildingpinesui.model.SprayPlan
import com.doc.wildingpinesui.model.SystemStatus
import java.util.*

/**
 * Used to change the state of the NUC.
 */
interface IStateChangeDataSource {
    /**
     * The status of the whole system, including the NUC and spraying systems.
     */
    val systemStatus: LiveData<SystemStatus>

    /**
     * The last time the status was updated from the NUC.
     */
    val lastStatusUpdate: LiveData<Date?>

    /**
     * @return whether the NUC was successfully set to flying mode.
     */
    suspend fun setNucToFlyingMode(sprayPlan: SprayPlan): Boolean

    /**
     * @return whether the NUC was successfully set to landed mode.
     */
    suspend fun setNucToLandedMode(): Boolean

    /**
     * @return whether the shutdown request was received by the NUC.
     */
    suspend fun requestNucShutdown(): Boolean

    /**
     * Update the status for the whole system by asking the NUC for a health check.
     */
    suspend fun updateSystemStatus()
}