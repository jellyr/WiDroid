﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;


namespace BasicWPF.MVVM
{
    public class FlowManager
    {
        #region Fields

        private static FlowManager _instance;

        private UnityContainer _container;
        private IMainWindow _mainWindow;
        private ICollection<IViewModel> _viewModels;

        #endregion

        #region Constructors

        private FlowManager()
        {
            ViewModels = new List<IViewModel>();
        }

        #endregion

        #region Public Methods

        public void ChangePage<TViewModel>() where TViewModel : IViewModel, new()
        {
            // If we are already on the same page as the button click, we don't change anything
            if (AppWindow.ViewModel.CurrentViewModel == null ||
                AppWindow.ViewModel.CurrentViewModel.GetType() != typeof(TViewModel))
            {
                foreach (IViewModel viewModel in ViewModels)
                {
                    // If an instance of the viewmodel already exists, we switch to that one
                    if (viewModel.GetType() == typeof(TViewModel))
                    {
                        AppWindow.ViewModel.CurrentViewModel = viewModel;
                        return;
                    }
                }

                // Else, we create a new instance of the viewmodel
                TViewModel newViewModel;
                if (_container != null)
                {
                    newViewModel = _container.Resolve<TViewModel>();
                }
                else
                {
                    newViewModel = new TViewModel();
                }
                AppWindow.ViewModel.CurrentViewModel = newViewModel;
                ViewModels.Add(newViewModel);
            }
        }

        #endregion

        #region Properties

        public static FlowManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FlowManager();
                }

                return _instance;
            }
        }

        public UnityContainer ViewModelContainer
        {
            get { return _container; }
            set
            {
                if (_container != value)
                {
                    _container = value;
                }
            }
        }

        public IMainWindow AppWindow
        {
            get { return _mainWindow; }
            set
            {
                if (_mainWindow != value)
                {
                    _mainWindow = value;
                }
            }
        }

        public ICollection<IViewModel> ViewModels
        {
            get { return _viewModels; }
            private set
            {
                if (_viewModels != value)
                {
                    _viewModels = value;
                }
            }
        }

        #endregion
    }
}
