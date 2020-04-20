using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using TMPro;
using Entities;

using Utilities.Events;

namespace UI.Menus
{
    public class UiNotificationsLog : UiWithScrollableItems, IPointerDownHandler
    {
        private static int MaxNotifications = 10;

        private static string MenuTitle = "NOTIFICATIONS LOG";
        private static string NewField = "NEW";

        public GameObject NofificationPrefab;

        private int NewNotificationsCount = 0;
        private string NewNotificationsCountString { get { return NewNotificationsCount + " " + NewField; } }

        private GameObject[] Notifications;
        private int NotificationsCount = 0;

        override public void Awake()
        {
            base.Awake();

            NewNotificationsCount = 0;
            NotificationsCount = 0;
            Notifications = new GameObject[MaxNotifications];

            TitleTextLeft.Text = MenuTitle;
            TitleTextRight.Text = NewNotificationsCountString;
        }

        // reset new notification count when mouse touches this window
        public void OnPointerDown(PointerEventData pointerEventData)
        {
            NewNotificationsCount = 0;
            TitleTextRight.Text = NewNotificationsCountString;
        }

        void Start()
        {
            Test();
        }

        void Test()
        {
            string longNotification =
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Donec odio. " +
                "Quisque volutpat mattis eros. Nullam malesuada erat ut turpis. " +
                "Suspendisse urna nibh, viverra non, semper suscipit, posuere a, pede." +
                "Donec nec justo eget felis facilisis fermentum. Aliquam porttitor mauris " +
                "sit amet orci. Aenean dignissim pellentesque felis." +
                "Morbi in sem quis dui placerat ornare. Pellentesque odio nisi, euismod in, " +
                "pharetra a, ultricies in, diam. Sed arcu. Cras consequat." +
                "Praesent dapibus, neque id cursus faucibus, tortor neque egestas auguae, " +
                "eu vulputate magna eros eu erat. Aliquam erat volutpat. " +
                "Nam dui mi, tincidunt quis, accumsan porttitor, facilisis luctus, metus.";

            AddNewNotification();
            AddNewNotification();
            AddNewNotification(longNotification);
            AddNewNotification(longNotification);
            AddNewNotification();
            AddNewNotification(longNotification);
        }

        public void AddNewNotification(string notification = "")
        {
            NewNotificationsCount++; // TODO: fix this on interaction
            TitleTextRight.Text = NewNotificationsCountString;

            // add a new notification text
            var notifObj = Instantiate(NofificationPrefab);

            // setup text fields
            var uiFieldNotification = notifObj.GetComponent<UiFieldNotification>();
            uiFieldNotification.Initialize(DateTime.Now.ToString("hh:mm:ss"), notification);

            // parent it to scrollable view
            notifObj.transform.parent = ContentRoot.transform;

            // add to notifications list
            // check if need to overwrite existing notification
            if (Notifications[NotificationsCount] != null)
                Destroy(Notifications[NotificationsCount]);

            Notifications[NotificationsCount] = notifObj;

            // increment notification count
            NotificationsCount = (NotificationsCount + 1) % MaxNotifications;
        }

        public void Update()
        {
            if (UnityEngine.Random.value < 0.01f)
                AddNewNotification();
        }

        // Update is called once per frame
    }

}