package com.doc.wildingpinesui.network.apiservices

import com.doc.wildingpinesui.model.ErrorResponse
import com.doc.wildingpinesui.model.HealthCheck
import com.doc.wildingpinesui.model.SprayPlan
import com.doc.wildingpinesui.model.SuccessResponse
import com.haroldadmin.cnradapter.NetworkResponse
import com.haroldadmin.cnradapter.NetworkResponseAdapterFactory
import com.jakewharton.retrofit2.adapter.kotlin.coroutines.CoroutineCallAdapterFactory
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST

/**
 * Used to make API calls that change the state of the NUC.
 */
interface IStateChangeApiService {
    @POST("fly")
    suspend fun setNucToFlyingMode(@Body sprayPlan: SprayPlan): NetworkResponse<SuccessResponse, ErrorResponse>

    @POST("land")
    suspend fun setNucToLandedMode(): NetworkResponse<SuccessResponse, ErrorResponse>

    @POST("shutdown")
    suspend fun requestNucShutdown(): NetworkResponse<SuccessResponse, ErrorResponse>

    @GET("health")
    suspend fun getHealthCheck(): NetworkResponse<HealthCheck, ErrorResponse>

    companion object {
        operator fun invoke(): IStateChangeApiService {
            return Retrofit
                .Builder()
                .baseUrl(ApiHostSettings.apiBaseUrl)
                .addCallAdapterFactory(CoroutineCallAdapterFactory())
                .addCallAdapterFactory(NetworkResponseAdapterFactory())
                .addConverterFactory(GsonConverterFactory.create())
                .build()
                .create(IStateChangeApiService::class.java)
        }
    }
}