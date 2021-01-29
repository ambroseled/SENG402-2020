package com.doc.wildingpinesui.network.datasources

import android.util.Log
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import com.doc.wildingpinesui.model.Nozzle
import com.doc.wildingpinesui.network.apiservices.INozzlesApiService
import com.haroldadmin.cnradapter.NetworkResponse

/**
 * Used to interact with the nozzles in the spray boom.
 */
class NozzlesDataSource(private val nozzlesApiService: INozzlesApiService) : INozzlesDataSource {
    private val _nozzlesInTheNuc: MutableLiveData<List<Nozzle>> = MutableLiveData(listOf())

    override val nozzlesInTheNuc: LiveData<List<Nozzle>>
        get() = _nozzlesInTheNuc

    override suspend fun fetchAvailableNozzles(): Boolean {
        val networkResponse = nozzlesApiService.getNozzles()
        return when(networkResponse) {
            is NetworkResponse.Success -> {
                _nozzlesInTheNuc.postValue(networkResponse.body)
                true
            }
            else -> {
                Log.w(this::class.java.simpleName, "Something went wrong getting the available nozzles from the API server.")
                false
            }
        }
    }

    override suspend fun updateNozzleStatuses(nozzles: List<Nozzle>): Boolean {
        val networkResponse = nozzlesApiService.updateNozzleStatuses(nozzles)
        return when(networkResponse) {
            is NetworkResponse.Success -> true
            else -> {
                Log.w(this::class.java.simpleName, "Something went wrong updating the nozzle statuses in the API server.")
                false
            }
        }
    }

}