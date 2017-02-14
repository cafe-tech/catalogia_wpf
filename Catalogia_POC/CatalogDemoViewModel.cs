/*
    Catalog WPF 0.7
    Copyright © 2016-2017, Michael Francisco / FCS. All rights reserved.
  
    Catalog WPF is licensed under the terms of the GPLv2
    <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>.

    This program is free software; you can redistribute it and/or modify 
    it under the terms of the GNU General Public License as published 
    by the Free Software Foundation; version 2 of the License.

    This script is distributed in the hope that it will be useful, but 
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
    or FITNESS FOR A PARTICULAR BUSINESS MODEL. See the GNU General Public License 
    for more details. 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;




namespace Catalogia_POC
{
    public class CatalogDemoViewModel : INotifyPropertyChanged
    {
        private ICommand m_ButtonCommand;

        CatalogDataContext md;
        CatalogObject _currentRecord;
        List<Category> _categories;
        public ObjectsInCollection ViewCollection { get; set; }
        public List<CategoriesLookUp> ViewCategories { get; set; }

        string connString = "server=<yourserver>;database=<database>;uid=<youruid>;pwd=<yourpwd>;";        

        int _currentRecordIndex = 0;
        string _recordCount = "";



        public ICommand ButtonCommand
        {
            get
            {
                return m_ButtonCommand;
            }
            set
            {
                m_ButtonCommand = value;
            }
        }
        public CatalogDemoViewModel()
        {
            ButtonCommand = new SendCommand(new Action<object>(Run));
            md = new CatalogDataContext(connString);

            ViewCollection = new ObjectsInCollection();
            ViewCategories = new List<CategoriesLookUp>();

            _categories = md.Categories.ToList();
            LoadCategories(_categories);
        }

        public void Run(object commandObj)
        {
            string command = commandObj.ToString();

            switch (command)
            {
                case "Reset":
                    ResetUI();
                    break;
                case "FirstPage":
                    FirstPage();
                    break;
                case "PrevPage":
                    PrevPage();
                    break;
                case "NextPage":
                    NextPage();
                    break;
                case "LastPage":
                    LastPage();
                    break;
                case "Create":
                    CreateObject();
                    break;
                case "Save":
                    SaveObject();
                    break;
                case "Update":
                    UpdateObject();
                    break;
                case "Delete":
                    DeleteObject();
                    break;
                default:
                    break;
            }

        }

        #region Functions

        private void ResetUI()
        {
            if (md != null)
            {
                md = null;
            }

            md = new CatalogDataContext(connString);

            //_currentRecordIndex = 0;
            //_currentRecord = md.Collections.FirstOrDefault().CatalogObjects.FirstOrDefault();
            //if (_currentRecord != null)
            //{
            //    LoadToControls(_currentRecord);
            //}

            FirstPage();
        }

        private void FirstPage()
        {
            if ( md.Collections.FirstOrDefault() != null)
            {
                _currentRecordIndex = 0;
                _currentRecord = md.Collections.FirstOrDefault().CatalogObjects[_currentRecordIndex];
            }
            if (_currentRecord != null)
            {
                LoadToControls(_currentRecord);
            }
            OnPropertyChanged("CurrentCategoryId");
            UpdatePageCounter();
        }

        private void PrevPage()
        {
            if (_currentRecordIndex > 0)
            {
                _currentRecordIndex = _currentRecordIndex - 1;
            }

            _currentRecord = md.Collections.FirstOrDefault().CatalogObjects[_currentRecordIndex];
            LoadToControls(_currentRecord);
            OnPropertyChanged("CurrentCategoryId");
            UpdatePageCounter();
        }

        private void NextPage()
        {
            if (_currentRecordIndex < md.Collections.FirstOrDefault().CatalogObjects.Count - 1)
            {
                _currentRecordIndex = _currentRecordIndex + 1;
            }

            _currentRecord = md.Collections.FirstOrDefault().CatalogObjects[_currentRecordIndex];
            LoadToControls(_currentRecord);
            OnPropertyChanged("CurrentCategoryId");
            UpdatePageCounter();
        }

        private void LastPage()
        {
            if (md.Collections.FirstOrDefault() != null)
            {
                _currentRecordIndex = md.Collections.FirstOrDefault().CatalogObjects.Count - 1;
                _currentRecord = md.Collections.FirstOrDefault().CatalogObjects[_currentRecordIndex];
            }
            if (_currentRecord != null)
            {
                LoadToControls(_currentRecord);
            }
            OnPropertyChanged("CurrentCategoryId");
            UpdatePageCounter();
        }

        private void UpdatePageCounter()
        {
            RecordCount = (_currentRecordIndex + 1).ToString() + " of " + md.Collections.FirstOrDefault().CatalogObjects.Count.ToString();
        }


        private void CreateObject()
        {
            string newObjectID = Guid.NewGuid().ToString();
            _currentRecord = new CatalogObject();
            _currentRecord.ObjectID = newObjectID;
            _currentRecord.CatalogCustomFields.Add(new Catalogia_POC.CatalogCustomFields());
            LoadToControls(_currentRecord);
        }

        private void SaveObject()
        {
            md.CatalogObjects.InsertOnSubmit(_currentRecord);
            md.SubmitChanges();

            ResetUI();
        }

        private void UpdateObjectOld()
        {
            // Method 1, use _currentRecord
            // Method 2, use c

            // md.Connection.Close();
            // md = new Catalogia_POC.CatalogDataContext(connString);

            LoadControlsToObject(ViewCollection);
            CatalogObject c = md.CatalogObjects.Where(x => x.ObjectID == _currentRecord.ObjectID).FirstOrDefault();

            c.ObjectName = _currentRecord.ObjectName;

            if (md != null)
            {
                // For 1st method, use _currentRecord instead of c
                // md.Refresh(RefreshMode.KeepCurrentValues, c);
                md.SubmitChanges();
            }
        }

        /// <summary>
        /// Seperate Data Context
        /// </summary>
        private void UpdateObject()
        {
            LoadControlsToObject(ViewCollection);

            // TODO: Do we need this?
            if (md != null)
            {
                ChangeSet cs = md.GetChangeSet();
                // md.Refresh(RefreshMode.KeepCurrentValues, _currentRecord);
                md.SubmitChanges();
            }            
        }

        private void DeleteObject()
        {
            if (md != null)
            { 
                md.CatalogObjects.DeleteOnSubmit(_currentRecord);
                md.SubmitChanges();
            }

            ResetUI();
        }


        private CatalogObject LoadControlsToObject(ObjectsInCollection or)
        {
            _currentRecord.CollectionID = or.Collection;
            _currentRecord.ObjectID = or.ObjectID;
            _currentRecord.ObjectName = or.ObjectName;
            _currentRecord.OtherName = or.OtherName;
            _currentRecord.OtherNumber = or.OtherNumber;

            _currentRecord.OldNumber = or.OldNumber;
            _currentRecord.Accession = or.Accession;
            _currentRecord.HomeLocation = or.HomeLocation;

            _currentRecord.Date = or.Date;
            _currentRecord.YearRange = or.YearRange;
            _currentRecord.CatalogDate = or.CatalogDate;
            _currentRecord.CatalogedBy = or.CatalogedBy;
            _currentRecord.StatusDate = or.StatusDate;
            _currentRecord.StatusBy = or.StatusBy;
            _currentRecord.Status = or.Status;


            bool needToCreateCcf = false;

            // If supplementary details exist, load and populate
            CatalogCustomFields ccf = _currentRecord.CatalogCustomFields.Where(x => x.ObjectID == _currentRecord.ObjectID).FirstOrDefault();
            if (ccf == null)
            {
                ccf = new Catalogia_POC.CatalogCustomFields();
                ccf.CustomFieldID = Guid.NewGuid().ToString();
                
                needToCreateCcf = true;                
            }

            ccf.CategoryID = _currentCategoryId;
            ccf.Description = or.Description;
            ccf.Collector = or.Collector;
            ccf.Title = or.Title;
            ccf.Artist = or.Artist;
            ccf.SignedName = or.SignedName;
            ccf.SigLocation = or.SigLocation;
            ccf.Medium = or.Medium;
            ccf.Material = or.Material;
            ccf.Technique = or.Technique;
            ccf.Culture = or.Culture;
            ccf.School = or.School;
            ccf.Accessories = or.Accessories;
            ccf.ImageSize = or.ImageSize;
            ccf.FrameSize = or.FrameSize;
            ccf.FrameDesc = or.FrameDesc;
            ccf.Provenance = or.Provenance;

            if (needToCreateCcf)
            {
                _currentRecord.CatalogCustomFields.Add(ccf);
            }

            return _currentRecord;
        }

        private void LoadToControls(CatalogObject or)
        {
            ViewCollection.Collection   = or.CollectionID;
            ViewCollection.ObjectID     = or.ObjectID;
            ViewCollection.ObjectName   = or.ObjectName;
            ViewCollection.OtherName    = or.OtherName;
            ViewCollection.OtherNumber  = or.OtherNumber;

            ViewCollection.OldNumber    = or.OldNumber;
            ViewCollection.Accession    = or.Accession;
            ViewCollection.HomeLocation = or.HomeLocation;

            ViewCollection.Date         = or.Date;
            ViewCollection.YearRange    = or.YearRange;
            ViewCollection.CatalogDate  = or.CatalogDate;
            ViewCollection.CatalogedBy  = or.CatalogedBy;
            ViewCollection.StatusDate   = or.StatusDate;
            ViewCollection.StatusBy     = or.StatusBy;
            ViewCollection.Status       = or.Status;

            if (or.CatalogCustomFields.Count > 0)
            {
                CurrentCategoryId           = or.CatalogCustomFields.FirstOrDefault().CategoryID;
                ViewCollection.Description  = or.CatalogCustomFields.FirstOrDefault().Description;
                ViewCollection.Collector    = or.CatalogCustomFields.FirstOrDefault().Collector;
                ViewCollection.Title        = or.CatalogCustomFields.FirstOrDefault().Title;
                ViewCollection.Artist       = or.CatalogCustomFields.FirstOrDefault().Artist;
                ViewCollection.SignedName   = or.CatalogCustomFields.FirstOrDefault().SignedName;
                ViewCollection.SigLocation  = or.CatalogCustomFields.FirstOrDefault().SigLocation;
                ViewCollection.Medium       = or.CatalogCustomFields.FirstOrDefault().Medium;
                ViewCollection.Material     = or.CatalogCustomFields.FirstOrDefault().Material;
                ViewCollection.Technique    = or.CatalogCustomFields.FirstOrDefault().Technique;
                ViewCollection.Culture      = or.CatalogCustomFields.FirstOrDefault().Culture;
                ViewCollection.School       = or.CatalogCustomFields.FirstOrDefault().School;
                ViewCollection.Accessories  = or.CatalogCustomFields.FirstOrDefault().Accessories;
                ViewCollection.ImageSize    = or.CatalogCustomFields.FirstOrDefault().ImageSize;
                ViewCollection.FrameSize    = or.CatalogCustomFields.FirstOrDefault().FrameSize;
                ViewCollection.FrameDesc    = or.CatalogCustomFields.FirstOrDefault().FrameDesc;
                ViewCollection.Provenance   = or.CatalogCustomFields.FirstOrDefault().Provenance;
            }
            else
            {
                if (ViewCategories.Count > 0)
                {
                    CurrentCategoryId = ViewCategories[0].CategoryId;
                }
            }
        }

        private void LoadCategories(List<Category> c)
        {
            if (ViewCategories.Count == 0)
            {
                foreach (Category cat in c)
                {
                    ViewCategories.Add(new CategoriesLookUp { CategoryId = cat.CategoryID, CategoryDesc = cat.CategoryDesc } );
                }
            }
        }

        private string getJSONDataFromFile()
        {
            string result = File.ReadAllText("testdata.txt");
            return result;
        }

        #endregion      // Functions

        public string RecordCount 
        {
            get
            {
                return _recordCount;
            }
            set
            {
                _recordCount = value;
                OnPropertyChanged("RecordCount");
            }
        }


        string _currentCategoryId;
        string _categoryDesc;

        public string CurrentCategoryId
        { 
            get            
            {
                return _currentCategoryId;
            }
            set
            {
                _currentCategoryId = value;

                OnPropertyChanged("CurrentCategoryId");
            }
        }

        public string CurrentCategoryDesc
        {
            get
            {
                return _categoryDesc;
            }
            set
            {
                _categoryDesc = value;
                OnPropertyChanged("CurrentCategoryDesc");
            }
        }

        #region INotifyPropertyChanged (for non-record Window objects)

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion


        public class ObjectsInCollection : INotifyPropertyChanged
        {
            #region private members

            private string _collectionID;
            private string _objectID;
            private string _objectName;
            private string _otherName;
            private string _otherNumber;
            private string _oldNumber;
            private string _accession;
            private string _homeLocation;
            private string _date;
            private string _yearRange;
            private string _catalogDate;
            private string _catalogedBy;
            private string _statusDate;
            private string _statusBy;
            private string _status;
            private string _category;
            private string _description;
            private string _collector;
            private string _title;
            private string _artist;
            private string _signedName;
            private string _sigLocation;
            private string _medium;
            private string _material;
            private string _technique;
            private string _culture;
            private string _school;
            private string _accessories;
            private string _imageSize;
            private string _frameSize;
            private string _frameDesc;
            private string _provenance;

            #endregion

            #region public members

            public string Collection
            {
                get
                {
                    return _collectionID;
                }
                set
                {
                    _collectionID = value;
                    OnPropertyChanged("Collection");
                }
            }
            public string ObjectID
            {
                get
                {
                    return _objectID;
                }
                set
                {
                    _objectID = value;
                    OnPropertyChanged("ObjectID");
                }
            }
            public string ObjectName
            {
                get
                {
                    return _objectName;
                }
                set
                {
                    _objectName = value;
                    OnPropertyChanged("ObjectName");
                }
            }
            public string OtherName
            {
                get
                {
                    return _otherName;
                }
                set
                {
                    _otherName = value;
                    OnPropertyChanged("OtherName");
                }
            }
            public string OtherNumber
            {
                get
                {
                    return _otherNumber;
                }
                set
                {
                    _otherNumber = value;
                    OnPropertyChanged("OtherNumber");
                }
            }
            public string OldNumber
            {
                get
                {
                    return _oldNumber;
                }
                set
                {
                    _oldNumber = value;
                    OnPropertyChanged("OldNumber");
                }
            }
            public string Accession
            {
                get
                {
                    return _accession;
                }
                set
                {
                    _accession = value;
                    OnPropertyChanged("Accession");
                }
            }
            public string HomeLocation
            {
                get
                {
                    return _homeLocation;
                }
                set
                {
                    _homeLocation = value;
                    OnPropertyChanged("HomeLocation");
                }
            }
            public string Date
            {
                get
                {
                    return _date;
                }
                set
                {
                    _date = value;
                    OnPropertyChanged("Date");
                }
            }
            public string YearRange
            {
                get
                {
                    return _yearRange;
                }
                set
                {
                    _yearRange = value;
                    OnPropertyChanged("YearRange");
                }
            }
            public string CatalogDate
            {
                get
                {
                    return _catalogDate;
                }
                set
                {
                    _catalogDate = value;
                    OnPropertyChanged("CatalogDate");
                }
            }
            public string CatalogedBy
            {
                get
                {
                    return _catalogedBy;
                }
                set
                {
                    _catalogedBy = value;
                    OnPropertyChanged("CatalogedBy");
                }
            }
            public string StatusDate
            {
                get
                {
                    return _statusDate;
                }
                set
                {
                    _statusDate = value;
                    OnPropertyChanged("StatusDate");
                }
            }
            public string StatusBy
            {
                get
                {
                    return _statusBy;
                }
                set
                {
                    _statusBy = value;
                    OnPropertyChanged("StatusBy");
                }
            }

            public string Status
            {
                get
                {
                    return _status;
                }
                set
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            }
            public string Category
            {
                get
                {
                    return _category;
                }
                set
                {
                    _category = value;
                    OnPropertyChanged("Category");
                }
            }
            public string Description
            {
                get
                {
                    return _description;
                }
                set
                {
                    _description = value;
                    OnPropertyChanged("Description");
                }
            }
            public string Collector
            {
                get
                {
                    return _collector;
                }
                set
                {
                    _collector = value;
                    OnPropertyChanged("Collector");
                }
            }
            public string Title
            {
                get
                {
                    return _title;
                }
                set
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
            public string Artist
            {
                get
                {
                    return _artist;
                }
                set
                {
                    _artist = value;
                    OnPropertyChanged("Artist");
                }
            }
            public string SignedName
            {
                get
                {
                    return _signedName;
                }
                set
                {
                    _signedName = value;
                    OnPropertyChanged("SignedName");
                }
            }
            public string SigLocation
            {
                get
                {
                    return _sigLocation;
                }
                set
                {
                    _sigLocation = value;
                    OnPropertyChanged("SigLocation");
                }
            }
            public string Medium
            {
                get
                {
                    return _medium;
                }
                set
                {
                    _medium = value;
                    OnPropertyChanged("Medium");
                }
            }
            public string Material
            {
                get
                {
                    return _material;
                }
                set
                {
                    _material = value;
                    OnPropertyChanged("Material");
                }
            }
            public string Technique
            {
                get
                {
                    return _technique;
                }
                set
                {
                    _technique = value;
                    OnPropertyChanged("Technique");
                }
            }
            public string Culture
            {
                get
                {
                    return _culture;
                }
                set
                {
                    _culture = value;
                    OnPropertyChanged("Culture");
                }
            }
            public string School
            {
                get
                {
                    return _school;
                }
                set
                {
                    _school = value;
                    OnPropertyChanged("School");
                }
            }
            public string Accessories
            {
                get
                {
                    return _accessories;
                }
                set
                {
                    _accessories = value;
                    OnPropertyChanged("Accessories");
                }
            }
            public string ImageSize
            {
                get
                {
                    return _imageSize;
                }
                set
                {
                    _imageSize = value;
                    OnPropertyChanged("ImageSize");
                }
            }
            public string FrameSize
            {
                get
                {
                    return _frameSize;
                }
                set
                {
                    _frameSize = value;
                    OnPropertyChanged("FrameSize");
                }
            }
            public string FrameDesc
            {
                get
                {
                    return _frameDesc;
                }
                set
                {
                    _frameDesc = value;
                    OnPropertyChanged("FrameDesc");
                }
            }
            public string Provenance
            {
                get
                {
                    return _provenance;
                }
                set
                {
                    _provenance = value;
                    OnPropertyChanged("Provenance");
                }
            }

            #endregion

            #region INotifyPropertyChanged

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion
        }

        public class CategoriesLookUp : INotifyPropertyChanged
        {
            string _CategoryId;
            string _CategoryDescription;
            string _SelectedCategoryId;
            public string CategoryId
            {
                get
                {
                    return _CategoryId;
                }
                set
                {
                    _CategoryId = value;
                    OnPropertyChanged("CategoryId");
                }
            }
            public string CategoryDesc
            {
                get
                {
                    return _CategoryDescription;
                }
                set
                {
                    _CategoryDescription = value;
                    OnPropertyChanged("CategoryDesc");
                }
            }
            public string SelectedCategoryId
            {
                get
                {
                    return _SelectedCategoryId;
                }
                set
                {
                    _SelectedCategoryId = value;
                    OnPropertyChanged("SelectedCategoryId");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public class MuseumCollections
        {
            public ObservableCollection<ObjectsInCollection> objectsInCollection { get; set; }
        }
    }
}
