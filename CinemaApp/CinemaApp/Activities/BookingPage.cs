using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Animation;
using CinemaApp.Model;
using CinemaApp.Resources.views;
using Newtonsoft.Json;

namespace CinemaApp.Activities
{
    public partial class MoviePageActivity : Activity
    {
        private List<SeatButton> _checkedSeats = new List<SeatButton>();

        private TextView _bookingPageMovieTitle;
        private Spinner _dateSpinner;
        private GridLayout _sessionsGrid;
        private GridLayout _seatsScheme;
        private Session _selectedSession;

        private void InitializeBookingPage()
        {
            _bookingPageMovieTitle.Text = _movie.Title;
            _dateSpinner.Adapter = new ArrayAdapter(this, Resource.Layout.SpinnerItem, _movie.ShowDays);
            _dateSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
                                         { PopulateSessionsGrid(_movie.ShowDays.Find(day => day.Date.DayOfYear ==
                                                                Convert.ToDateTime((e.View as TextView).Text).DayOfYear)); };
            _buttonConfirmBooking.Click += (object sender, EventArgs e) =>
                                                {
                                                    Intent intent = new Intent(this, typeof(ConfirmBookingActivity));
                                                    intent.PutExtra("MovieID", _movie.ID);
                                                    intent.PutExtra("SessionID", _selectedSession.ID);
                                                    intent.PutExtra("SelectedSeats", JsonConvert.SerializeObject(_checkedSeats.Select(s =>
                                                                                                                 _seatsScheme.IndexOfChild(s))));
                                                    StartActivity(intent);
                                                };
            //Добавить потом, когда нормально запилю сеансы в бд
            //PopulateSessionsGrid(_movie.ShowDays.Find(day => day.Date.DayOfYear == DateTime.Now.DayOfYear));
            if (_movie.ShowDays.Count > 0)
                PopulateSessionsGrid(_movie.ShowDays[0]);


            for (int i = 0; i < _seatsScheme.ChildCount; i++)
                (_seatsScheme.GetChildAt(i) as SeatButton).Click += SeatButton_Click;
        }

        private void SeatButton_Click(object sender, EventArgs e)
        {
            if ((sender as SeatButton).Checked)
                _checkedSeats.Add(sender as SeatButton);
            else
                _checkedSeats.Remove(sender as SeatButton);
            UpdateBookingInfo();
        }

        private void ClearSeats()
        {
            for (int i = 0; i < _seatsScheme.ChildCount; i++)
            {
                _seatsScheme.GetChildAt(i).Enabled = true;
                (_seatsScheme.GetChildAt(i) as Resources.views.SeatButton).Checked = false;

                if (_selectedSession.BookedSeats.Exists(s => (s.Row - 1) + (s.Number - 1) == i))
                {
                    _seatsScheme.GetChildAt(i).Enabled = false;
                    _seatsScheme.GetChildAt(i).SetBackgroundColor(Android.Graphics.Color.DarkGray);
                }
            }
        }

        private void PopulateSessionsGrid(Day day)
        {
            bool isAnySessionSelected = false;
            _sessionsGrid.RemoveAllViews();
            foreach (var session in day.Sessions)
            {
                Button tile = CreateSessionTile(session);
                if (day.Date.DayOfYear == DateTime.Now.DayOfYear 
                 && Convert.ToDateTime(session.Time).TimeOfDay < DateTime.Now.TimeOfDay)
                {
                    tile.Enabled = false;
                    tile.SetBackgroundColor(Android.Graphics.Color.Gray);
                }
                if (isAnySessionSelected == false && tile.Enabled == true)
                {
                    SelectSession(tile, session);
                    isAnySessionSelected = true;
                }
                _sessionsGrid.AddView(tile);
                
            }
        }

        private Button CreateSessionTile(Session session)
        {
            Button tile = new Button(this);
            tile.Text = session.Time;
            tile.SetTextColor(Android.Graphics.Color.Black);
            tile.SetBackgroundColor(Android.Graphics.Color.White);
            tile.TextSize = 20;
            tile.SetPadding(60, 0, 60, 0);
            GridLayout.LayoutParams layoutParams = new GridLayout.LayoutParams();
            layoutParams.LeftMargin = layoutParams.RightMargin = 40;
            layoutParams.TopMargin = 20;
            tile.LayoutParameters = layoutParams;
            tile.Touch += (object sender, View.TouchEventArgs e) => { SelectSession(tile, session); };
            return tile;
        }

        private void SelectSession(View tile, Session session)
        {
            for (int i = 0; i < _sessionsGrid.ChildCount; i++)
            {
                View view = _sessionsGrid.GetChildAt(i);
                if (view.Enabled)
                    view.SetBackgroundColor(Android.Graphics.Color.White);
            }
                
            tile.SetBackgroundColor(Android.Graphics.Color.LightGreen);
            _selectedSession = session;
            ClearSeats();
            _checkedSeats.Clear();
            UpdateBookingInfo();
        }

        private void UpdateBookingInfo()
        {
            StringBuilder seatsList = new StringBuilder("Seats:\n");
            foreach (SeatButton seat in _checkedSeats.OrderBy(s => _seatsScheme.IndexOfChild(s)))
                seatsList.AppendLine($"Row {_seatsScheme.IndexOfChild(seat)/10 + 1}, Seat {_seatsScheme.IndexOfChild(seat) % 10 + 1}");
            _seatsList.Text = seatsList.ToString();
            _totalPrice.Text = $"{_checkedSeats.Count * 70} UAH";
        }

        public override void OnBackPressed()
        {
            if (_bookingPage.Visibility == ViewStates.Invisible)
                base.OnBackPressed();
            else
            {
                int centerX = (_buttonBookTicket.Left + _buttonBookTicket.Right) / 2;
                int centerY = (_buttonBookTicket.Top + _buttonBookTicket.Bottom) / 2;
                float radius = Math.Max(_mainContainer.Width, _mainContainer.Height);

                _mainContainer.Visibility = ViewStates.Visible;
                Animator hide = ViewAnimationUtils.CreateCircularReveal(_bookingPage, centerX, centerY, radius, 0);
                hide.AnimationEnd += (object sender, EventArgs e) => { _bookingPage.Visibility = ViewStates.Invisible; };
                hide.SetDuration(600);
                hide.Start();
            }
        }
    }
}