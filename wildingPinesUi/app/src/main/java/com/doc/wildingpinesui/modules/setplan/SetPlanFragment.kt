package com.doc.wildingpinesui.modules.setplan

import android.app.AlertDialog
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.CheckBox
import androidx.core.view.children
import androidx.fragment.app.Fragment
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.doc.wildingpinesui.R
import com.doc.wildingpinesui.model.SprayPlan
import com.google.android.material.snackbar.Snackbar
import kotlinx.android.synthetic.main.set_plan_section.*
import kotlinx.android.synthetic.main.sync_files_section.*
import kotlinx.android.synthetic.main.sync_files_section.setPlanSprayPlansLinearLayout
import kotlinx.coroutines.launch
import org.kodein.di.KodeinAware
import org.kodein.di.android.x.closestKodein
import org.kodein.di.generic.instance

/**
 * The section for the 'Set a plan' section of the app.
 */
class SetPlanFragment : Fragment(), KodeinAware {
    companion object {
        fun newInstance() = SetPlanFragment()
    }
    override val kodein by closestKodein()
    private val viewModelFactory: SetPlanViewModelFactory by instance()
    private lateinit var viewModel: SetPlanViewModel

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.set_plan_section, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)
        viewModel = ViewModelProvider(this, viewModelFactory).get(SetPlanViewModel::class.java)
        bindUi()
    }

    /**
     * Populate the UI and define the logic to handle user activity.
     */
    private fun bindUi() = lifecycleScope.launchWhenCreated {
        watchAvailableSprayPlans()
        viewModel.fetchAvailableSprayPlansInNuc()
        setUpLogicForFlyButton()
        setUpLogicForLandButton()
    }

    /**
     * When the available spray plans are updated, update the UI.
     */
    private fun watchAvailableSprayPlans() {
        viewModel.sprayPlansInNuc.observe(viewLifecycleOwner, Observer {
            Log.d(this::class.java.simpleName, "Received ${it.size} spray plans in NUC")
            populateSprayPlansInNuc(it)
        })
    }

    /**
     * Tell the NUC to be ready to fly.
     */
    private fun setUpLogicForFlyButton() {
        flyButton.setOnClickListener { view ->
            val selectedPlan = getSelectedPlan()
            if (selectedPlan == null) {
                AlertDialog.Builder(this@SetPlanFragment.context).create().apply {
                    setMessage("Please select a spray plan before trying to fly.")
                }.show()
                return@setOnClickListener
            }

            Log.d(this::class.java.simpleName, "Setting NUC to flying mode with plan: $selectedPlan")

            lifecycleScope.launch {
                val sprayPlan = SprayPlan(selectedPlan)
                val didSetToFlyingMode = viewModel.setNucToFlyingMode(sprayPlan)
                if (didSetToFlyingMode) {
                    Snackbar.make(view, "NUC is ready to fly.", Snackbar.LENGTH_LONG).show()
                } else {
                    AlertDialog.Builder(this@SetPlanFragment.context).create().apply {
                        setMessage("Could not set the NUC to fly.")
                    }.show()
                }
            }
        }
    }

    /**
     * Tell the NUC that we have landed.
     */
    private fun setUpLogicForLandButton() {
        landButton.setOnClickListener { view ->
            lifecycleScope.launch {
                val didSetToLandedMode = viewModel.setNutToLandedMode()
                if (didSetToLandedMode) {
                    Snackbar.make(view, "NUC is now landed", Snackbar.LENGTH_LONG).show()
                } else {
                    AlertDialog.Builder(this@SetPlanFragment.context).create().apply {
                        setMessage("Could not set the NUC to landed")
                    }.show()
                }
            }
        }
    }

    /**
     * Display the available spray plans in the UI.
     */
    private fun populateSprayPlansInNuc(sprayPlans: List<SprayPlan>) {
        view?.let {view ->
            setPlanSprayPlansLinearLayout.removeAllViews() // removes all children, if any
            sprayPlans.forEach {
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
     * @return the filepath for the first selected spray plan.
     */
    private fun getSelectedPlan(): String? {
        val plansCheckBoxes: List<CheckBox> = setPlanSprayPlansLinearLayout.children
            .filter { it is CheckBox }
            .map { it as CheckBox }
            .toList()
        return plansCheckBoxes
            .find { it.isChecked }?.text.toString()
    }
}
