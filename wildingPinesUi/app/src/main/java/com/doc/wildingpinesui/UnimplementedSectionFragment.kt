package com.doc.wildingpinesui

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import com.doc.wildingpinesui.sidebar.SidebarSections
import kotlinx.android.synthetic.main.item_detail.view.*

/**
 * A fragment for a section in the app that is not yet implemented.
 */
class UnimplementedSectionFragment : Fragment() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
    }

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val rootView = inflater.inflate(R.layout.item_detail, container, false)
        rootView.item_detail.text = "To be implemented"
        return rootView
    }
}
