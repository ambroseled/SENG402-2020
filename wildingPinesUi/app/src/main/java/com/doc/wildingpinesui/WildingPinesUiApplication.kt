package com.doc.wildingpinesui

import android.app.Application
import com.doc.wildingpinesui.network.apiservices.ISprayPlansApiService
import com.doc.wildingpinesui.modules.setplan.SetPlanViewModelFactory
import com.doc.wildingpinesui.modules.syncfiles.SyncFilesViewModelFactory
import com.doc.wildingpinesui.modules.testnozzles.TestNozzlesViewModelFactory
import com.doc.wildingpinesui.network.apiservices.INozzlesApiService
import com.doc.wildingpinesui.network.apiservices.ISprayReportsApiService
import com.doc.wildingpinesui.network.apiservices.IStateChangeApiService
import com.doc.wildingpinesui.network.datasources.*
import org.kodein.di.Kodein
import org.kodein.di.KodeinAware
import org.kodein.di.android.x.androidXModule
import org.kodein.di.generic.bind
import org.kodein.di.generic.instance
import org.kodein.di.generic.provider
import org.kodein.di.generic.singleton

/**
 * The Android application.
 * Used to declare dependency injection objects.
 */
class WildingPinesUiApplication: Application(), KodeinAware {
    /**
     * Used tutorial in
     * https://www.youtube.com/watch?v=YVtXgPeuoMA&list=PLB6lc7nQ1n4jTLDyU2muTBo8xk0dg0D_w&index=5
     * for dependency injection.
     */
    override val kodein = Kodein.lazy {
        import(androidXModule(this@WildingPinesUiApplication))

        // API services
        bind() from singleton { ISprayPlansApiService() }
        bind() from singleton { IStateChangeApiService() }
        bind() from singleton { ISprayReportsApiService() }
        bind() from singleton { INozzlesApiService() }

        // data sources
        bind<ISyncFilesDataSource>() with singleton { SyncFilesDataSource(instance(), instance()) }
        bind<IStateChangeDataSource>() with singleton { StateChangeDataSource(instance()) }
        bind<INozzlesDataSource>() with singleton { NozzlesDataSource(instance()) }

        // ViewModel factories
        bind() from provider { TestNozzlesViewModelFactory(instance()) }
        bind() from provider { SyncFilesViewModelFactory(instance()) }
        bind() from provider { SetPlanViewModelFactory(instance(), instance()) }
        bind() from provider { MainActivityViewModel(instance()) }
    }
}