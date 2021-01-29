package com.doc.wildingpinesui

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.doc.wildingpinesui.network.datasources.IStateChangeDataSource

class MainActivityViewModelFactory(private val stateChangeDataSource: IStateChangeDataSource) : ViewModelProvider.NewInstanceFactory() {
    @Suppress("UNCHECKED_CAST")
    override fun <T : ViewModel?> create(modelClass: Class<T>): T {
        return MainActivityViewModel(stateChangeDataSource) as T
    }
}