﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KantineApp.Entity;
using KantineApp.Interface;
using Xamarin.Forms;
using System.Diagnostics;

namespace KantineApp.Pages
{
    public partial class NewMenuPage : ContentPage
    {

        //readonly IRepository _repo = Factory.GetRepository;
        private IServiceGateway _serviceGateway = Factory.GetServiceGateway;
        private MenuEntity menuToCreate;
        private Button takePhotoBtn = new Button();
        private Button pickPhotoBtn = new Button();
        private Button removeDishBtn = new Button();
        private Dish _tmpDish = null;
        ICameraHandler _cameraHandler;

        public NewMenuPage()
        {
            InitializeComponent();
            menuToCreate = new MenuEntity();
            menuToCreate.Dishes = new List<Dish>();
            _cameraHandler = DependencyService.Get<ICameraHandler>();
            _cameraHandler.AddPhotoTakenEventHandler(PhotoReceived); // Callback
            DishWrapperStack.Children.Add(NewDishStack(new Dish()));
        }

        {
            //DisplayAlert("Info", "Pick a photo is NOT IMPLEMENTED YET!", "OK");
            await Navigation.PushModalAsync(new GalleryPage());
        }

        private void TakeNewPhoto(Dish dish)
        {
            if (_tmpDish != null)
                Debug.WriteLine("one try");
            _tmpDish = dish;
            _cameraHandler.TakePhoto();
        }

        public void PhotoReceived(string fileName)
        {
            _tmpDish.Image = fileName;
            if (_tmpDish != null)
            {
                _tmpDish = null;
            }
            Debug.WriteLine(fileName);
        }


        /// <summary>
        /// Create a new Dish stacklayout, with text-entry and buttons for image options.
        /// </summary>
        /// <returns></returns>
        public StackLayout NewDishStack(Dish dish)
        {
            takePhotoBtn = new Button()
            {
                BackgroundColor = Color.FromHex("#313030"),
                WidthRequest = 50
            };
            pickPhotoBtn = new Button()
            {
                BackgroundColor = Color.FromHex("#313030"),
                WidthRequest = 50
            };
            removeDishBtn = new Button()
            {
                BackgroundColor = Color.FromHex("#313030"),
                WidthRequest = 50
            };

            var dishStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.StartAndExpand
            };
            var dishName = new Entry()
            {
                Placeholder = "Rettens navn",
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            dishStack.Children.Add(dishName);
            dish.Name = dishName.Text;

            takePhotoBtn.Image = "camera.png";
            pickPhotoBtn.Image = "file.png";
            removeDishBtn.Text = "Tis";
            dishStack.Children.Add(takePhotoBtn);
            dishStack.Children.Add(pickPhotoBtn);
            dishStack.Children.Add(removeDishBtn);


            takePhotoBtn.Clicked += (sender, args) => { TakeNewPhoto(dish); };
            removeDishBtn.Clicked += (sender, args) => { RemoveDish(dish, dishStack); };
            dishName.TextChanged += (sender, args) => { SaveDishName(dish, dishName.Text); };

            menuToCreate.Dishes.Add(dish);
            return dishStack;
        }
        private void RemoveDish(Dish dish, StackLayout dishStack)
        {
            DishWrapperStack.Children.Remove(dishStack);
            menuToCreate.Dishes.Remove(dish);
        }

        private void SaveDishName(Dish dish, string dishName)
        {
            dish.Name = dishName;
        }

        /// <summary>
        /// When add new dish is clicked, add new Dish stacklayout to parent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddDishStak(object sender, EventArgs e)
        {
            DishWrapperStack.Children.Add(NewDishStack(new Dish()));
        }

        /// <summary>        
        /// create a new menu with a list of dishes and a given date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateNewMenu_Button_OnClicked(object sender, EventArgs e)
        {
            menuToCreate.Date = dPicker.Date;
            _serviceGateway.Create(menuToCreate);
            DishWrapperStack.Children.Clear();
            menuToCreate = new MenuEntity();
            menuToCreate.Dishes = new List<Dish>();
            DishWrapperStack.Children.Add(NewDishStack(new Dish()));
        }

        /**protected override void OnAppearing()
        {
            Debug.WriteLine("On appering");
            base.OnAppearing();
            DishWrapperStack.Children.Clear();
            _dishes.Clear();
            DishWrapperStack.Children.Add(NewDishStack());
        }**/

    }
}
