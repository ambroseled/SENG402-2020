package com.doc.wildingpinesui.modules.syncfiles

import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.AdapterView
import android.widget.ArrayAdapter
import android.widget.CheckBox
import androidx.core.view.children
import androidx.fragment.app.Fragment
import androidx.lifecycle.*
import com.doc.wildingpinesui.R
import com.doc.wildingpinesui.model.SprayPlan
import com.google.android.material.snackbar.Snackbar
import kotlinx.android.synthetic.main.sync_files_section.*
import kotlinx.android.synthetic.main.sync_files_section.view.*
import kotlinx.coroutines.launch
import org.kodein.di.KodeinAware
import org.kodein.di.android.x.closestKodein
import org.kodein.di.generic.instance

/**
 * The section for the 'Sync files' section of the app.
 */
class SyncFilesFragment : Fragment(), KodeinAware {
    override val kodein by closestKodein()
    private lateinit var viewModel: SyncFilesViewModel
    private val viewModelFactory: SyncFilesViewModelFactory by instance()

    private val _selectedUsbName = MutableLiveData<String>("")
    val selectedUsbName: LiveData<String>
        get() = _selectedUsbName

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.sync_files_section, container, false)
    }

    /**
     * Populate the dropdown for the available spray plan sources e.g. available USBs.
     */
    private fun populateAvailableSprayPlanSources(view: View?, sources: List<String>) {
        view?.let {
            ArrayAdapter<String>(
                view.context, android.R.layout.simple_spinner_item, sources
            ).also { arrayAdapter ->
                // Specify the layout to use when the list of choices appears
                arrayAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
                // Apply the adapter to the spinner
                sprayPlansLocationSpinner.adapter = arrayAdapter
            }
        }
    }

    /**
     * Create a checkbox for each report in the scroll view.
     */
    private fun populateAvailableSprayPlans(view: View?, plans: List<SprayPlan>) {
        view?.let {
            setPlanSprayPlansLinearLayout.removeAllViews() // removes all children, if any
            plans.forEach {
                val checkBox = CheckBox(view.context).apply {
                    text = it.name
                    setTextColor(resources.getColor(R.color.textColorPrimary, view.context.theme))
                    buttonTintList = resources.getColorStateList(R.color.boringWhite, view.context.theme)
                }
                setPlanSprayPlansLinearLayout.addView(checkBox)
            }
        }
    }

    /**
     * Create a checkbox for each report in the scroll view.
     */
    private fun populateAvailableReports(view: View?, plans: List<String>) {
        view?.let {
            reportsLinearLayout.removeAllViews() // removes all children, if any
            plans.forEach {
                val checkBox = CheckBox(view.context).apply {
                    text = it
                    setTextColor(resources.getColor(R.color.textColorPrimary, view.context.theme))
                    buttonTintList = resources.getColorStateList(R.color.boringWhite, view.context.theme)
                }
                reportsScrollView.reportsLinearLayout.addView(checkBox)
            }
        }
    }

    /**
     * @return the spray plans that are checked
     */
    private fun getSelectedSprayPlans(): List<String> {
        val checkBoxes: Sequence<CheckBox> = setPlanSprayPlansLinearLayout.children
            .filter { it is CheckBox }.map { it as CheckBox }
        return checkBoxes
            .filter { it.isChecked }
            .map { it.text.toString() }
            .toList()
    }

    /**
     * @return the reports that are checked
     */
    private fun getSelectedReports(): List<String> {
        val checkBoxes: Sequence<CheckBox> = reportsLinearLayout.children
            .filter { it is CheckBox }.map { it as CheckBox }
        return checkBoxes
            .filter { it.isChecked }
            .map { it.text.toString() }
            .toList()
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)
        viewModel = ViewModelProvider(this, viewModelFactory)
            .get(SyncFilesViewModel::class.java)
        bindUi()
    }

    /**
     * Populate the UI and define the logic to handle user activity.
     */
    private fun bindUi() = lifecycleScope.launchWhenCreated {
        watchAvailableUsbDrives()
        watchAvailableSprayReports()
        watchSelectedUsbDrive()
        watchAvailableSprayPlans()

        viewModel.fetchCurrentAvailableUsbDrives()

        setUpLogicForSprayLocationsSpinner()
        setUpLogicForUploadToNucButton()
        setUpLogicForUploadReportsToUsbButton()
        setUpLogicForSyncAllFilesButton()
    }

    /**
     * Update the available spray sources in the UI when they are updated.
     */
    private fun watchAvailableUsbDrives() {
        viewModel.downloadedAvailableUsbDrives.observe(viewLifecycleOwner, Observer {
            populateAvailableSprayPlanSources(view, it.map { usbDrive ->  usbDrive.name })
        })
    }

    /**
     * Update the available spray reports in the UI when they are updated.
     */
    private fun watchAvailableSprayReports() {
        viewModel.sprayReportsInNuc.observe(viewLifecycleOwner, Observer {
            populateAvailableReports(view, it.map { report -> report.name })
        })
    }

    /**
     * Watch the selected USB name to fetch available spray plans when it's updated.
     */
    private fun watchSelectedUsbDrive() {
        selectedUsbName.observe(viewLifecycleOwner, Observer {
            if (it.isNotEmpty()) {
                lifecycleScope.launch {
                    viewModel.fetchSprayPlansInUsb(it)
                }
            } else {
                populateAvailableSprayPlanSources(view, listOf())
            }
        })
    }

    /**
     * Update the UI for the available spray plans when they are updated.
     */
    private fun watchAvailableSprayPlans() {
        viewModel.downloadedSprayPlansInUsbDrive.observe(viewLifecycleOwner, Observer {
            populateAvailableSprayPlans(view, it)
        })
    }

    /**
     * Update the selected spray plan source when one is selected to let the rest of the UI know.
     */
    private fun setUpLogicForSprayLocationsSpinner() {
        sprayPlansLocationSpinner.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
            override fun onItemSelected(
                parent: AdapterView<*>?,
                view: View?,
                position: Int,
                id: Long
            ) {
                val sprayPlanSource = parent?.getItemAtPosition(position).toString()
                Log.d(SyncFilesFragment::class.java.simpleName, "Selected spray plan source: $sprayPlanSource")
                _selectedUsbName.postValue(sprayPlanSource)
            }

            override fun onNothingSelected(parent: AdapterView<*>?) {
                _selectedUsbName.postValue("")
            }
        }
    }

    /**
     * Tell the ViewModel to import the selected spray plans to the NUC.
     */
    private fun setUpLogicForUploadToNucButton() {
        uploadToNucButton.setOnClickListener {
            lifecycleScope.launch {
                val filepaths = getSelectedSprayPlans()
                viewModel.importSprayPlansFromUsbToNuc(selectedUsbName.value.orEmpty(), filepaths)
            }
        }
    }

    /**
     * Export the selected reports to the currently selected USB.
     */
    private fun setUpLogicForUploadReportsToUsbButton() {
        uploadReportsToUsbButton.setOnClickListener {
            lifecycleScope.launch {
                val reportNames = getSelectedReports()
                viewModel.exportSprayReportsFromNucToUsb(selectedUsbName.value.orEmpty(), reportNames)
            }
        }
    }

    /**
     * Import all the spray plans to the NUC, and export all the reports to the USB.
     */
    private fun setUpLogicForSyncAllFilesButton() {
        syncAllFilesButton.setOnClickListener {view ->
            lifecycleScope.launch {
                viewModel.importAllSprayPlansFromUsbToNuc(selectedUsbName.value.orEmpty())
                viewModel.exportAllSprayReportsFromNucToUsb(selectedUsbName.value.orEmpty())
                Snackbar.make(view, "Synced all spray plans to NUC and reports to USB", Snackbar.LENGTH_LONG).show()
            }
        }
    }
}