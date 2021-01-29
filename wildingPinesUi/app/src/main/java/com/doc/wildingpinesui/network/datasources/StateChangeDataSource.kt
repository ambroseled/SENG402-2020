package com.doc.wildingpinesui.network.datasources

import android.util.Log
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import com.doc.wildingpinesui.model.SprayPlan
import com.doc.wildingpinesui.model.SystemStatus
import com.doc.wildingpinesui.network.apiservices.IStateChangeApiService
import com.haroldadmin.cnradapter.NetworkResponse
import java.util.*

/**
 * Used to change the state of the NUC.
 */
class StateChangeDataSource(private val stateChangeApiService: IStateChangeApiService) : IStateChangeDataSource {
    private val _systemStatus = MutableLiveData(SystemStatus.NUC_UNREACHABLE)
    private val _lastStatusUpdate: MutableLiveData<Date?> = MutableLiveData(null)

    override val systemStatus: LiveData<SystemStatus>
        get() = _systemStatus

    override val lastStatusUpdate: LiveData<Date?>
        get() = _lastStatusUpdate

    override suspend fun setNucToFlyingMode(sprayPlan: SprayPlan): Boolean {
        val networkResponse = stateChangeApiService.setNucToFlyingMode(sprayPlan)
        return when (networkResponse) {
            is NetworkResponse.Success -> {
                Log.d(this::class.java.simpleName, "Set NUC to flying mode. Message from NUC: ${networkResponse.body.message}")
                true
            }
            is NetworkResponse.ServerError -> {
                Log.e(this::class.java.simpleName, "ServerError Could not set NUC to flying mode because ${networkResponse.body?.message}.")
                false
            }
            is NetworkResponse.NetworkError  -> {
                Log.e(this::class.java.simpleName, "NetworkError Could not set NUC to flying mode.", networkResponse.error)
                false
            }
            is NetworkResponse.UnknownError -> {
                Log.e(this::class.java.simpleName, "UnknownError Could not set NUC to flying mode.", networkResponse.error)
                false
            }
        }
    }

    override suspend fun setNucToLandedMode(): Boolean {
        val networkResponse = stateChangeApiService.setNucToLandedMode()
        return when(networkResponse) {
            is NetworkResponse.Success -> true
            else -> false
        }
    }

    override suspend fun requestNucShutdown(): Boolean {
        val networkResponse = stateChangeApiService.requestNucShutdown()
        return when (networkResponse) {
            is NetworkResponse.Success -> {
                Log.i(this::class.java.simpleName, "Shutdown response from NUC: ${networkResponse.body.message}")
                true
            }
            else -> {
                Log.e(this::class.java.simpleName, "Error when shutting down system: $networkResponse")
                false
            }
        }
    }

    override suspend fun updateSystemStatus() {
        val networkResponse = stateChangeApiService.getHealthCheck()
        when (networkResponse) {
            is NetworkResponse.Success -> {
                _systemStatus.postValue(SystemStatus.OK)
                _lastStatusUpdate.postValue(networkResponse.body.timestamp)
            }
            else -> _systemStatus.postValue(SystemStatus.NUC_UNREACHABLE)
        }
    }
}