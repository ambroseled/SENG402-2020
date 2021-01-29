package com.doc.wildingpinesui.modules.setplan

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.doc.wildingpinesui.model.SprayPlan
import com.doc.wildingpinesui.network.datasources.IStateChangeDataSource
import com.doc.wildingpinesui.network.datasources.ISyncFilesDataSource
import kotlinx.coroutines.launch

/**
 * Stores and updates data used by the UI in the 'Set a plan' section of the app.
 */
class SetPlanViewModel(
    private val syncFilesDataSource: ISyncFilesDataSource,
    private val stateChangeDataSource: IStateChangeDataSource
) : ViewModel() {
    private val _sprayPlansInNuc = MutableLiveData<List<SprayPlan>>()
    val sprayPlansInNuc: LiveData<List<SprayPlan>>
        get() = _sprayPlansInNuc

    init {
        viewModelScope.launch {
            fetchAvailableSprayPlansInNuc()
            syncFilesDataSource.sprayPlansInNuc.observeForever {
                _sprayPlansInNuc.postValue(it)
            }
        }
    }

    suspend fun fetchAvailableSprayPlansInNuc() {
        syncFilesDataSource.fetchAvailableSprayPlansInNuc()
    }

    /**
     * @return whether the operation succeeded.
     */
    suspend fun setNucToFlyingMode(sprayPlan: SprayPlan): Boolean {
        return stateChangeDataSource.setNucToFlyingMode(sprayPlan)
    }

    /**
     * @return whether the operation succeeded.
     */
    suspend fun setNutToLandedMode(): Boolean {
        return stateChangeDataSource.setNucToLandedMode()
    }
}