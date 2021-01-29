package com.doc.wildingpinesui.modules.setplan

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.doc.wildingpinesui.network.datasources.IStateChangeDataSource
import com.doc.wildingpinesui.network.datasources.ISyncFilesDataSource

/**
 * Used to construct a [SetPlanViewModel] for dependency injection.
 */
class SetPlanViewModelFactory(
    private val syncFilesDataSource: ISyncFilesDataSource,
    private val stateChangeDataSource: IStateChangeDataSource
) : ViewModelProvider.NewInstanceFactory() {
    @Suppress("UNCHECKED_CAST")
    override fun <T : ViewModel?> create(modelClass: Class<T>): T {
        return SetPlanViewModel(syncFilesDataSource, stateChangeDataSource) as T
    }
}