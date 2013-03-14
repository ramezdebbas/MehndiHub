using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace FoodVariable.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : FoodVariable.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get { return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }



        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "In Pakistan and India every woman knows that bride is incomplete without applying beautiful and stunning mehndi designs on her hands, feet and arms. These days, applying mehndi becomes popular fashion. Every year, numerous mehndi designs are coming for women and young girls. In this post, we are presenting latest and exclusive mehndi designs 2013 for women. Women and girls can apply these mehndi designs on their hands, arms and feet. All mehndi designs 2013 are simply stunning and magnificent. These mehndi designs 2013 include different types of designs like floral designs, peacock designs and many more. If we talk about these mehndi designs then some mehndi designs are extremely beautiful but difficult. So women can apply them with the help of professional mehndi artist. On the other hand, some of them are simple then even girls can easily apply them without taking any help.");

            var group1 = new SampleDataGroup("Group-1",
                 "Arabic Mehndi Design",
                 "Group Subtitle: 1",
                 "Assets/DarkGray.png",
                 "Group Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs. In this style we can use different styles of mehndi like Black mehndi is used as outline, fillings with the normal henna mehndi. We can also include sparkles as a final coating to make the henna design more attractive.");

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item1",
                 "Bridal Mehndi Design",
                 "Bridal Mehndi Design",
                 "Assets/HubPage/HubpageImage2.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "Bridal mehndi designs for hands of this year are worth seeing. They are different from the all past designs of mehndi designs or patterns. Bridal mehndi designs for hands are mostly used by the young girls for wedding parties. As bridal mehndi designs for hands are closely associated with wedding that means it is only related to bride but they are also copied by all the girls and women for party make up or attending marriage ceremony. Bridal mehndi designs for hands 2013 are very beautiful and unique. They make some one different and unique and more beautiful than others. Bridal mehndi designs for hands is pattern of different colors consist of different patterns of flowers, petals and veins. Sometimes artificial patterns are also created for mahndi to be applied on hands. Here some latest bridal mehndi designs for hands 2013 are uploaded for them who show keen interest in the application of mehndi. Bridal mehndi designs for hands are nicely made with much wisdom and consideration and with also taking the requirements of all the conditions. ",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item2",
                 "Back Hand Mehndi Design",
                 "Back Hand Mehndi Design",
                 "Assets/HubPage/HubpageImage3.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "Mehndi means henna, but it is most commonly used in the west as a term for the designs painted on the hands, feet, or other parts of the body, using henna as the stain.  This makes what is sometimes called a temporary tattoo, though by definition, a tattoo is put into the skin using a needle.Mehndi are widely used throughout southern and western Asia and Africa.  Sometimes this is to protect the skin from the sun, sometimes for weddings, and sometimes for simple beautification.  It depends on the culture and local area customs.",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item3",
                 "Fully Green Mehndi",
                 "Fully Green Mehndi",
                 "Assets/HubPage/HubpageImage4.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "There are many recipes for henna mehndi, but the most common use some sort of adhesive to hold the henna on the skin long enough to stain it, and acid to set it.  Sometimes other ingredients containing tannins or other natural dyes, such as coffee or indigo, are added to the mix to make it darker.  A plain, natural henna stain is light orange-brown.  Black pekoe tea will make it a little richer brown.  Freshly ground coffee will make it a very rich brown.  Beet juice will make it red.  Indigo will give it a bluish tint, and black walnut hulls will make it dark brown.  To make a black stain safely takes indigo and black walnut hulls.  If it has anything else in it, be very wary.  With any chemical or dye, even if it's natural, take care to do an allergy test before marking larger areas of the body.",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item4",
                 "Full Front Hands Mehndi",
                 "Full Front Hands Mehndi",
                 "Assets/HubPage/HubpageImage5.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "Very few people in the west have time for mehndi, so quite often non toxic watercolor markers are used since they can be washed off with soap and water at the end of the day.  For a more durable mark, surgical markers or semi permanent makeup pens are used.  There are also tattoo gel pens and tattoo markers made for kids that provide a fairly durable mark, but are very washable.  You can find many body painting supplies and skin pens and markers in our store.",
                 69,
                 70,
                 group1));

            group1.Items.Add(new SampleDataItem("Landscape-Group-1-Item5",
                 "Shadow Mehndi Style",
                 "Shadow Mehndi Style",
                 "Assets/HubPage/HubpageImage6.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "Henna is very popular, but most women these days just don't have the luxury of sitting for hours and hours.  Also, some designs one may want to wear on the weekends, may not be appropriate for work.  So semi permanent makeup is a good alternative.To begin, you will need an exfoliating, moisturizing cleanser (mildly abraisive, not alpha-hydroxy or otherwise too acidic)and the cosmetic pen in the desired color.Gently scrub your hands or whatever area you will be applying the design to, and allow enough time to dry. Draw the design carefully, and allowing for some bleeding, as the pen tips are quite wet.",
                 69,
                 35,
                 group1));

            

            this.AllGroups.Add(group1);

            var group2 = new SampleDataGroup("Group-2",
                "Indian Mehndi Design",
                "Group Subtitle: 2",
                "Assets/DarkGray.png",
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group2.Items.Add(new SampleDataItem("Big-Group-2-Item1",
                "Full Hands Mehndi Design",
                "Full Hands Mehndi Desing",
                "Assets/HubPage/HubpageImage7.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Arm mehndi style is a pattern of mehndi used for hands and full arm. It may be up to half arm or till elbows. Some times it is enhanced to the desired length. Actually the arm mehndi style starts from hand tip and then it is stretched to the elbow in back ward direction. Arm mehndi style contains many patterns. It may have a single pattern or multi pattern mehndi style. In arm mehndi style different color techniques are also now use d in the application of mehndi. There are some images that show the beauty of arm mehndi style. Mehndi is typically concerned with the woman make up so it is always noticed by the women. It is the only fashion which every type of economic class can follow it and can remain up to date with the latest mehndi designs. Arm mehndi styles’ images are shown. Look their beauty and unique value. They really enhance the beauty of woman. It completes the make up.",
                69,
                70,
                group2));

            group2.Items.Add(new SampleDataItem("Landscape-Group-2-Item2",
                "Latest Hindi Mehndi Design",
                "Latest Hindi Mehndi Design",
                "Assets/HubPage/HubpageImage8.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "The top latest Indian mehndi designs 0f 2013 are so much popular among the mehndi designs in the fashion industry related to the fashion of mehndi. These designs are so cool and attractive. The top latest Indian mehndi designs 0f 2013 are so much attractive and also so much easy from the application on the hands and feet. The top latest Indian mehndi designs 0f 2013 have full collection of all types patterns of hands and feet. The top latest Indian mehndi designs 0f 2013 have full hand mehndi designs and full arm mehndi designs and smart mehndi designs for hands and feet and legs. The top latest Indian mehndi designs 0f 2013 have become the first priority of the women as they are unique in design and contain a lot of new and trendy and stylish patterns. The full collection of mehndi designs from hands and feet are in the The top latest Indian mehndi designs 0f 2013. The top latest Indian mehndi designs 0f 2013 is simply the full collection and the most accurate and the best one for the women as they are so simple to apply and so much beautiful to look. ",
                69,
                35,
                group2));

            group2.Items.Add(new SampleDataItem("Medium-Group-2-Item3",
                "Full Hands & Legs Mehndi Style",
                "Full Hands & Legs Mehndi Style",
                "Assets/HubPage/HubpageImage9.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Fashion has many aspects with different angles. Concept of Mehndi is old and everyone is familiar with its use and importance. Sometimes among some people it becomes an essential and important part of their life. Mehndi is a pattern that is applied on hands and legs. It is obtained from natural resources. Mehndi is pasted on hands and legs. There are seen many designs for full hand mehndi 2013 and legs designs etc. latest and most recent mehndi designs are here given. Ladies really love to use mehndi on their full hands. It makes them confident in completing their makeup. Full hand mehndi designs 2013 are frequently asked by the women now a day as they need them for party and wedding ceremonies. In the South Asia the makeup is considered not complete without Mehndi. So by knowing the importance of full hand mehndi and its demand among the women, some latest pattern of mehndi are shown.",
                41,
                41,
                group2));

            group2.Items.Add(new SampleDataItem("Medium-Group-2-Item4",
                "Rajastaan Mehndi Style",
                "Rajastaan Mehndi Style",
                "Assets/HubPage/HubpageImage09.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Mehndi designs for the hands of Rajasthan are so popular as every one knows about its beauty and attraction of the designs belong to Rajasthan. Mehndi designs for the hands of Rajasthan are so much beautiful because mehndi designs are closely associated with the east of the world. The designs of mehndi of India are so much popular all around the world due its historical and traditional beauty and trendy stylish look and attractive and quite nice patterns. Mehndi designs for the hands of Rajasthan are adopted with out any thinking every in the world and they are highly praised due to its high name and fame in the designs of mehndi. Mehndi designs for the hands of Rajasthan have new and attracrive patterns for the hands and full arm. Mehndi designs for the hands of Rajasthan have stylish patterns for hands and arms. They contain many different patterns for the different women. Mehndi designs for the hands of Rajasthan are famous because of their full and exotic patterns. Normally Mehndi designs for the hands of Rajasthan are heavy patterns than other designs. ",
                41,
                41,
                group2));

            
            this.AllGroups.Add(group2);


            

            var group3 = new SampleDataGroup("Group-3",
               "Pakistani",
               "Group Subtitle: 2",
               "Assets/DarkGray.png",
               "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group3.Items.Add(new SampleDataItem("Big-Group-3-Item1",
                "Lahori Mehndi Design",
                "Lahori Mehndi Design",
                "Assets/HubPage/HubpageImage10.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "The latest and new mehndi designs for hands of 2013 are so consciously seen and searched by the women now a day. These are the days in which the women are so much eager to have some thing new in the fashion. They are looking for the new deigns of dresses from the new fashion year and they are looking for the new fashion make up for the year. Similarly concept of fashion with out the designs of mehndi can never be completed by the women. So they are so much eager to know about the new and stylish and the latest designs of mehndi for the hands of 2013. Latest and the new mehndi designs for the hands of 2013 are available now as the fashion designs in the fashion market and they are published by top fashion magazines. Some magazines have some thing special from these articles. These designs are of a lot of variety as they are so much in number and designs. Only Latest and the new mehndi designs for the hands of 2013 are so much in number that they are so much and so beautiful to chose easily. ",
                69,
                70,
                group3));

            group3.Items.Add(new SampleDataItem("Landscape-Group-3-Item2",
                "Punjabi Mehndi Design",
                "Punjabi Mehndi Design",
                "Assets/HubPage/HubpageImage11.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Mehndi is a unique type of fashion that can never be minimized and lasted as other fashions are done. Fashion has a trend and it starts at certain time and lasts at certain time but mehndi is such a huge fashion that is always increasing and it is due to history and solid traditional back ground. Mehndi has been used for the centuries among the women of all around the world. It will not be false if we say that the first most high fashion was mehndi and now also its grown form is not less than any other full fashion of the entire world. Mehndi is a beautiful pattern of color on the hands and feet. It is applied with much care and in much time. Now a day glitters are also used along with the mehndi as it is the way to make more beautiful the mehndi. Glitters mehndi designs for the parties are so much beautiful as they can be seen in the images of the latest photos of the collection of the latest mehndi designs for the hands and feet. And use of glitters also depict that it enhance the beauty of the mehndi for so many times.",
                69,
                35,
                group3));

            group3.Items.Add(new SampleDataItem("Medium-Group-3-Item3",
                "Stylish Mehndi Design",
                "Stylish Mehndi Design",
                "Assets/HubPage/HubpageImage12.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "The latest and stylish mehndi designs for the feet can be searched easily by the women though any mean. Mehndi designs are always the keen interest of women. Mehndi designs which are quite new and latest in the fashion market are searched frequently by the fashion artist as well as the women who are so much fond of mehndi. Mehndi designs are available from hands and feet. They are of different types with respect to their patterns and size and style. The latest and stylish mehndi designs for the feet are the beautiful patterns of mehndi that can be applied on feet as the requirement of the consumer. These patterns are of different styles and size. But the beauty of the mehndi is quite energetic and attractive. The latest and stylish mehndi designs for the feet are the special and beautiful patterns of mehndi applied on a part of feet extended to the desired length. The difference in the latest and stylish mehndi designs for the feet is dependant of the event on which Mehndi is applied. Let us see some of the most beautiful patterns of the latest and stylish mehndi designs for the feet in the images. ",
                41,
                41,
                group3));
            group3.Items.Add(new SampleDataItem("Medium-Group-3-Item4",
               "Latest Mehndi Design",
               "Latest Mehndi Design",
               "Assets/HubPage/HubpageImage13.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "Mehndi as a fashion accessory is so clear in the makeup. In the fashion value of mehndi has no doubt about it even a minute. Mehndi has been a so old tradition as a fashion. Mehndi has been used for centuries ago among the women of the world. It is supposed to be the first fashion accessory in the fashion make up. Now in the latest fashion its value is not minimized by the modern fashion but it has made its place more perfect and better than earlier. There has been so much improvement in the Mehndi. Mehndi as a fashion accessory is used in the every make up. It is largely used in the fashion industry by the fashion artists of our industry. Recently Mehndi as a fashion accessory has become so much vital as its natural beauty and secure fashion treatment. Mehndi as a fashion accessory is so much nice and perfectly suitable for everyone. Mehndi as a fashion accessory has no side effects as other fashion accessories may have. Let us see the beauty of Mehndi as a fashion accessory. ",
               41,
               41,
               group3));

            this.AllGroups.Add(group3);


         



            var group4 = new SampleDataGroup("Group-4",
               "The Most Famous",
               "Group Subtitle: 2",
               "Assets/DarkGray.png",
               "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item1",
               "Karva Chout Mehndi Design",
               "Karva Chout Mehndi Design",
               "Assets/HubPage/HubpageImage14.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "Fashion is always has been the keen choice and interest of the women. This has been proved by their search about the latest trend of fashion in everything and women want everything latest on their special days and these special days come many times in a month. Fashion is also supposed as the expression of feelings and taste and tradition and culture by any nation. Sometimes fashion has to be engaged in the tradition rites. Mehndi has been so much popular among the ladies, as its importance in the culture and traditional fashion. It has been the part of trendy fashion. Women are always looking for the latest mehndi designs daily as they want change daily and want to look change on every event. There for they prefer to use different mehndi designs on different occasions. Mehndi designs F and mehndi designs for Karva Chauth have been so much popular now a day. It is the design of mehndi used by the women on their religious event. This occurs in every month on the full moon day. Women are used to apply the designs and patterns of mehndi made especially for this day.",
               41,
               41,
               group4));

            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item2",
                "Famous Design Ever",
                "Famous Design Ever",
                "Assets/HubPage/HubpageImage15.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "As the spring starts, the women begin to search the latest mehndi designs for themselves. They visit fashion designers and go to have fashion magazines from where they could have the latest designs of mehndi. As it is connected with eastern fashion so the designs are also related to this region. The designs associated with the region and states of India are always very important and are searched and adopted frequently and blindly. The designs of India are associated with its states. Significance of Mehndi designs in India is that it gives all types of designs for all parts of body. Significance of Mehndi designs in India is due to its quite unique and new type of designs in the mehndi designs. All the most wanted mehndi designs all around the world are said to relate with the Indian designs and it is the Significance of Mehndi designs in India. Significance of Mehndi designs in India is due to its old tradition and civilization as mehndi is said to be originated from the same place of India so it also make India special in the mehndi designs and it is true bout the mehndi designs of India that they are really supreme. ",
                41,
                41,
                group4));
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item3",
               "Event Specific",
               "Event Specific",
               "Assets/HubPage/HubpageImage16.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "Different events of applying Mehndi are different with respect to their importance in our country. Mehndi is mostly so common in eastern countries. In Pakistan and India it is so much common and demandable and value able in the fashion or common make up. Make up is always remain the weak point of the women of every time and every place and every society and country. In Pakistan and India concept of make up is totally incomplete without mehndi. Present season of spring is one of the Different events of applying Mehndi to apply mehndi and enjoy its charm and beauty with full luxury. It is the most beautiful natural thing in the make up ever. Different events of applying Mehndi contain many events like bridal make up and party make up. In Pakistan mehndi is used at the religious events like Eid etc. Different events of applying Mehndi made the demand of mehndi so much high as every women and every girl wants to have mehndi and apply it on her hands and other desired part of body for show and make herself beautiful and attractive. ",
               41,
               41,
               group4));
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item4",
               "Easy Arm Mehndi",
               "Easy Arm Mehndi",
               "Assets/HubPage/HubpageImage17.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "Easy arm mendi is the application of mehndi on full arm till elbows at least. Mehndi is the in fact application various, beautiful and charming pattern of different color schemes. Easy arm mehndi designs 2013 are so prefer able among the young girls as they feel happy and please to apply them on their full hand and arm and legs. Easy arm mehndi pattern is the most unique and attractive mehndi pattern ever found or made. Easy arm mehndi designs 2013 are looked and searched by not only the young girls but also ladies and women also included. Easy arm mehndi designs 2013 are available in the form of print and soft copy. They are published in magazines. Beautiful, new and attractive Easy arm mehndi designs are pasted here in this article to be help full for the women and for girls to find it easily and apply it for the party and wedding ceremony. Easy arm mehndi 2013 is exotic, superb, nice looking and attractive as everyone can see its unique beauty by the given pictures.",
               41,
               41,
               group4));
            this.AllGroups.Add(group4);



        }
    }
}
