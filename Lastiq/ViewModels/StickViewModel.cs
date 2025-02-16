﻿using System.Collections.Generic;
using Lastiq.Infrastructure.Commands.Base;
using Lastiq.Models;
using Lastiq.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Lastiq.Infrastructure.Commands.Base;
using SticksyProtocol;


namespace Lastiq.ViewModels
{
    public class StickViewModel : ViewModel
    {
        //---------------------------------------------------------------------
        private static MainWindowViewModel MainViewModel
            => (MainWindowViewModel) Application.Current.MainWindow.DataContext;
        //---------------------------------------------------------------------
        public StickModel Stick { get; set; } = new StickModel(creatorId: 0);
        //---------------------------------------------------------------------
        private SolidColorBrush _color = StickModel.DefColor;
        public SolidColorBrush Color
        { 
            get => Stick.Color;
            set
            {
                if (Set(ref _color, value))
                    Stick.Color = _color;
            }
        }

        public void FromStick(Stick stick)
        {
            Stick.Title = stick.title;
            Stick.Contents = new List<IStickContentFrontend>();
            foreach (var content in stick.content)
            {
                if (content is TextContent tc) Stick.Contents.Add(new TextContentFrontend(tc));
                else if (content is CheckboxContent cbc) Stick.Contents.Add(new CheckboxContentFrontend(cbc));
            }
            Stick.DateTime = stick.date;
            Stick.Color.Color = (Color)ColorConverter.ConvertFromString(stick.color);
            Stick.Id = stick.id;
            Stick.CreatorId = stick.idCreator;
            //Stick.FriendsId = stick.Visiters; Conflict
            Stick.Tags.AddRange(stick.tags);
        }

        public Stick ToStick()
        {
            Stick stick = new Stick(Stick.Id, Stick.CreatorId);
            stick.title = Stick.Title;
            stick.content = new List<IStickContent>();
            foreach (var content in Stick.Contents)
            {
                if (content is TextContentFrontend tc) stick.content.Add(tc.ToSticksyTextContent());
                else if (content is CheckboxContentFrontend cbc) stick.content.Add(cbc.ToSticksyCheckBoxContent());
            }
            stick.date = Stick.DateTime;
            stick.color = Stick.Color.ToString();
            stick.id = Stick.Id;
            stick.idCreator = Stick.CreatorId;
            //stick.Visiters = Stick.FriendsId; Conflict
            stick.tags = Stick.Tags;
            return stick;
        }

        //---------------------------------------------------------------------
        #region Command : DeleteStickCommand

        private AppCommand _DeleteStickCommand;
        public AppCommand DeleteStickCommand
        {
            get => _DeleteStickCommand ?? (_DeleteStickCommand = new ActionCommand(DeleteStick));
            set => _DeleteStickCommand = value;
        }

        private void DeleteStick(object obj) => MainViewModel.RemoveSticker(this);

        #endregion Command : DeleteStickCommand
        //---------------------------------------------------------------------
        #region Command : EditStickCommand

        private AppCommand _EditStickCommand;
        public AppCommand EditStickCommand
        {
            get => _EditStickCommand ?? (_EditStickCommand = new ActionCommand(EditStick));
            set => _EditStickCommand = value;
        }

        private void EditStick(object obj)
        {
            ReadOnly = !ReadOnly;
            if(!ReadOnly) MainViewModel.StickerEdited(this);
        }

        #endregion Command : EditStickCommand
        //---------------------------------------------------------------------
        #region bool : StickReadOnly

        private bool _ReadOnly = true;
        public bool ReadOnly
        {
            get => _ReadOnly;
            set => Set(ref _ReadOnly, value);
        }

        #endregion bool : StickReadOnly
        //---------------------------------------------------------------------
        #region Command : CheckboxClickCommand

        private AppCommand _CheckboxClickCommand;
        public AppCommand CheckboxClickCommand
        {
            get => _CheckboxClickCommand ?? (_CheckboxClickCommand = new ActionCommand(CheckboxClick));
            set => _CheckboxClickCommand = value;
        }

        private void CheckboxClick(object obj)
        {
            var checkbox = obj as CheckboxContent;
            checkbox.isChecked = !checkbox.isChecked;
        }

        #endregion Command : CheckboxClickCommand
        //---------------------------------------------------------------------
        #region Command : AddTagCommand

        private AppCommand _AddTagCommand;
        public AppCommand AddTagCommand
        {
            get => _AddTagCommand ?? (_AddTagCommand = new ActionCommand(AddTag));
            set => _AddTagCommand = value;
        }

        private void AddTag(object obj)
        {
            string newTag = "NewTag";
            Stick.Tags.Add(newTag);
            MainViewModel.TagsChanged(this);

            Stick.Tags.Add(newTag);
        }

        #endregion Command : AddTagCommand
        //---------------------------------------------------------------------
        #region Command : DelTagCommand

        private AppCommand _DelTagCommand;
        public AppCommand DelTagCommand
        {
            get => _DelTagCommand ?? (_DelTagCommand = new ActionCommand(DelTag));
            set => _DelTagCommand = value;
        }

        private void DelTag(object obj)
        {
            Stick.Tags.Remove(obj as string);
        }

        #endregion Command : DelTagCommand
        //---------------------------------------------------------------------
    }
}
