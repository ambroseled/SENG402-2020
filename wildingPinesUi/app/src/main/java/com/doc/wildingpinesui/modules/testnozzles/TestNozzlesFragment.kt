package com.doc.wildingpinesui.modules.testnozzles

import android.os.Bundle
import android.view.ContextThemeWrapper
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.LinearLayout
import android.widget.Switch
import android.widget.TextView
import androidx.fragment.app.Fragment
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.doc.wildingpinesui.R
import com.doc.wildingpinesui.model.Nozzle
import com.google.android.material.snackbar.Snackbar
import kotlinx.android.synthetic.main.test_nozzles_section.*
import kotlinx.coroutines.launch
import org.kodein.di.KodeinAware
import org.kodein.di.android.x.closestKodein
import org.kodein.di.generic.instance

/**
 * The section for the 'Test nozzles' section of the app.
 */
class TestNozzlesFragment : Fragment(), KodeinAware {
    override val kodein by closestKodein()
    private lateinit var viewModel: TestNozzlesViewModel
    private val viewModelFactory: TestNozzlesViewModelFactory by instance()

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.test_nozzles_section, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)
        viewModel = ViewModelProvider(this, viewModelFactory)
            .get(TestNozzlesViewModel::class.java)
        bindUi()
    }

    /**
     * Populate the UI and define the logic to handle user activity.
     */
    private fun bindUi() {
        lifecycleScope.launchWhenCreated {
            refreshNozzlesFromNuc()
        }

        watchNozzlesFromViewModel()

        setUpLogicForTurnAllOnButton()
        setUpLogicForTurnAllOffButton()
    }

    /**
     * Whenever the nozzles in the view model update, update the UI to match.
     */
    private fun watchNozzlesFromViewModel() {
        refreshNozzlesFromNuc()
        viewModel.nozzlesInTheNuc.observe(viewLifecycleOwner, Observer {
            populateNozzleSwitches(it)
        })
    }

    /**
     * Request spray module to turn on all the nozzles.
     */
    private fun setUpLogicForTurnAllOnButton() {
        turnAllOnButton.setOnClickListener {
            updateAllNozzles(true)
        }
    }

    /**
     * Request spray module to turn off all the nozzles.
     */
    private fun setUpLogicForTurnAllOffButton() {
        turnAllOffButton.setOnClickListener {
            updateAllNozzles(false)
        }
    }

    /**
     * Make the UI for the nozzle switches and set up the logic for when they are interacted with.
     */
    private fun populateNozzleSwitches(nozzles: List<Nozzle>) {
        nozzleButtonsLinearLayout.removeAllViews() // clear children, if any
        nozzles.forEach { nozzle ->
            val verticalLinearLayout = LinearLayout(context).apply {
                orientation = LinearLayout.VERTICAL
            }
            val nozzleNameTextView = TextView(context).apply {
                text = nozzle.displayName
            }
            val newContext = ContextThemeWrapper(context, R.style.AppSwitchTheme)
            val nozzleSwitch = Switch(newContext).apply {
                isChecked = nozzle.shouldBeSpraying
                setOnCheckedChangeListener { buttonView, isChecked ->
                    run {
                        updateNozzleStatus(nozzle, isChecked)
                    }
                }
            }

            verticalLinearLayout.addView(nozzleNameTextView)
            verticalLinearLayout.addView(nozzleSwitch)
            nozzleButtonsLinearLayout.addView(verticalLinearLayout)
        }
    }

    /**
     * Called when a nozzle's status is updated. Tell the API server.
     */
    private fun updateNozzleStatus(nozzle: Nozzle, shouldBeSpraying: Boolean) {
        val newNozzles = viewModel.nozzlesInTheNuc.value.orEmpty().toList()
        newNozzles.forEach {
            if (it.id == nozzle.id) {
                it.shouldBeSpraying = shouldBeSpraying
            }
        }
        lifecycleScope.launch {
            val sentRequestToArduino = viewModel.updateNozzleStatuses(newNozzles)
            view?.let {
                val message = if (sentRequestToArduino) "Sent request to boom controller" else "Something went wrong when asking the boom controller to update the nozzles"
                Snackbar.make(view!!, message, Snackbar.LENGTH_LONG).show()
            }
            refreshNozzlesFromNuc()
        }
    }

    /**
     * Updates all nozzles to the same status and sends them to the API server at once.
     */
    private fun updateAllNozzles(shouldBeSpraying: Boolean) {
        val newNozzles = viewModel.nozzlesInTheNuc.value.orEmpty().toList()
        newNozzles.forEach { it.shouldBeSpraying = shouldBeSpraying }
        lifecycleScope.launch {
            val sentRequestToArduino = viewModel.updateNozzleStatuses(newNozzles)
            view?.let {
                val action = if (shouldBeSpraying) "turn on" else "turn off"
                val message = if (sentRequestToArduino) "Sent request to boom controller" else "Something went wrong when asking boom controller to $action all nozzles"
                Snackbar.make(view!!, message, Snackbar.LENGTH_LONG).show()
            }
            refreshNozzlesFromNuc()
        }
    }

    /**
     * Requests for the nozzles, and their status, to be updated using the nozzle statuses in the
     * NUC.
     */
    private fun refreshNozzlesFromNuc() {
        lifecycleScope.launch {
            val succeeded = viewModel.fetchNozzlesInTheNuc()
            val message = if (succeeded) "Updated the nozzles from the NUC" else "Something went wrong updating the nozzles from the NUC"
            view?.let {
                Snackbar.make(it, message, Snackbar.LENGTH_LONG).show()
            }
        }
    }
}