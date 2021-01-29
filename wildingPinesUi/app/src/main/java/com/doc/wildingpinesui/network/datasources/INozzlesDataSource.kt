package com.doc.wildingpinesui.network.datasources

import androidx.lifecycle.LiveData
import com.doc.wildingpinesui.model.Nozzle

/**
 * Used to interact with the nozzles in the spray boom.
 */
interface INozzlesDataSource {

    /**
     * The nozzles that are in the NUC.
     */
    val nozzlesInTheNuc: LiveData<List<Nozzle>>

    /**
     * Get the nozzles available in the spray boom from the API server.
     * @return whether the operation succeeded.
     */
    suspend fun fetchAvailableNozzles(): Boolean

    /**
     * Update the statuses of the nozzles in the API service.
     * @return true if the boom was requested to update the nozzle statuses, false if something
     * went wrong.
     */
    suspend fun updateNozzleStatuses(nozzles: List<Nozzle>): Boolean
}