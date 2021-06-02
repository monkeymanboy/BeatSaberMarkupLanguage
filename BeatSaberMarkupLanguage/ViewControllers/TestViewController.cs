using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Notify;
using HMUI;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    //[ViewDefinition("BeatSaberMarkupLanguage.Views.test.bsml")]
    public class TestViewController : BSMLResourceViewController//BSMLAutomaticViewController
    {
        public override string ResourceName => "BeatSaberMarkupLanguage.Views.test.bsml";

        [UIValue("header")]
        public string HeaderText
        {
            get => headerText;
            set
            {
                headerText = value;
                NotifyPropertyChanged();
            }
        }
        public string headerText = "Header comes from code!";

        [UIValue("some-number")]
        public int SomeNumber
        {
            get => someNumber;
            set
            {
                someNumber = value;
                NotifyPropertyChanged();
            }
        }
        public int someNumber = 2342531;

        [UIValue("lorem-ipsum")]
        private string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tincidunt augue interdum velit euismod in pellentesque. A iaculis at erat pellentesque adipiscing commodo elit at imperdiet. Ultrices sagittis orci a scelerisque purus semper eget. Semper risus in hendrerit gravida rutrum quisque non tellus orci. Rhoncus mattis rhoncus urna neque. Quisque sagittis purus sit amet. Eleifend quam adipiscing vitae proin. Pharetra et ultrices neque ornare aenean euismod elementum nisi. Quis hendrerit dolor magna eget est lorem ipsum dolor. Venenatis lectus magna fringilla urna porttitor rhoncus dolor purus. Lectus magna fringilla urna porttitor. Mi eget mauris pharetra et ultrices neque ornare aenean. Facilisi etiam dignissim diam quis enim lobortis scelerisque. Morbi tempus iaculis urna id volutpat lacus laoreet non. In hac habitasse platea dictumst quisque sagittis purus sit amet. Ultricies integer quis auctor elit sed vulputate mi. In tellus integer feugiat scelerisque. " +
            "Aliquet nec ullamcorper sit amet risus nullam eget felis eget.Nunc eget lorem dolor sed viverra ipsum nunc aliquet bibendum.Malesuada proin libero nunc consequat interdum varius sit amet mattis. Augue eget arcu dictum varius duis. Tempor nec feugiat nisl pretium fusce id velit ut.Feugiat in fermentum posuere urna nec tincidunt praesent semper feugiat. Tincidunt ornare massa eget egestas purus. Aliquet lectus proin nibh nisl condimentum id venenatis a.Tincidunt praesent semper feugiat nibh sed pulvinar proin. Viverra tellus in hac habitasse. Eget mi proin sed libero enim sed faucibus. Sed augue lacus viverra vitae congue eu consequat ac.Semper eget duis at tellus at urna condimentum. Sagittis orci a scelerisque purus semper eget duis at tellus. Placerat in egestas erat imperdiet sed. Amet est placerat in egestas erat." +
            "Tincidunt tortor aliquam nulla facilisi.Ornare aenean euismod elementum nisi quis eleifend.Viverra maecenas accumsan lacus vel facilisis volutpat est velit egestas. Nullam ac tortor vitae purus faucibus ornare suspendisse. Tincidunt dui ut ornare lectus sit amet.Id semper risus in hendrerit gravida rutrum quisque. Ornare arcu dui vivamus arcu felis bibendum ut tristique et. Tristique nulla aliquet enim tortor at. Nec tincidunt praesent semper feugiat nibh. Sed tempus urna et pharetra." +
            "Faucibus ornare suspendisse sed nisi lacus sed viverra tellus.Consectetur libero id faucibus nisl tincidunt eget nullam non.Lorem ipsum dolor sit amet consectetur adipiscing elit ut aliquam. Lacus vel facilisis volutpat est velit egestas dui id.Nunc sed id semper risus.Massa tempor nec feugiat nisl pretium. Nibh cras pulvinar mattis nunc sed blandit libero volutpat sed. Nibh praesent tristique magna sit.Adipiscing at in tellus integer feugiat scelerisque. Cursus metus aliquam eleifend mi in nulla.Sollicitudin ac orci phasellus egestas tellus rutrum tellus pellentesque.Elit sed vulputate mi sit amet. Nunc lobortis mattis aliquam faucibus purus in massa tempor. Egestas purus viverra accumsan in nisl.Eget aliquet nibh praesent tristique.Commodo viverra maecenas accumsan lacus vel facilisis volutpat est velit. Facilisis sed odio morbi quis commodo." +
            "Amet consectetur adipiscing elit duis.Vel pretium lectus quam id leo in vitae turpis massa.Fringilla phasellus faucibus scelerisque eleifend donec pretium vulputate sapien.Sed arcu non odio euismod.Sed tempus urna et pharetra pharetra massa massa ultricies.Sed vulputate odio ut enim blandit. Ac tortor vitae purus faucibus ornare suspendisse sed nisi lacus. Ut sem viverra aliquet eget sit amet tellus cras.Nulla pellentesque dignissim enim sit amet venenatis.Dolor morbi non arcu risus quis varius.Non tellus orci ac auctor augue mauris.At consectetur lorem donec massa sapien faucibus.Urna cursus eget nunc scelerisque.In tellus integer feugiat scelerisque varius morbi enim nunc faucibus. Id cursus metus aliquam eleifend mi in nulla.Ut porttitor leo a diam sollicitudin tempor id eu.";

        [UIComponent("test-external")]
        public TextMeshProUGUI buttonText;

        [UIComponent("list")]
        public CustomListTableData tableData;

        [UIValue("contents")]
        public List<object> contents
        {
            get
            {
                List<object> list = new List<object>();
                list.Add(new TestListObject("first", false));
                list.Add(new TestListObject("second", true));
                list.Add(new TestListObject("third", true));
                return list;
            }
        }
        [UIValue("contents2")]
        public List<object> contents2
        {
            get
            {
                List<object> list = new List<object>();
                for(int i = 0; i < 30; i++)
                {
                    list.Add(new TestListObject($"item {i}", false));
                }
                return list;
            }
        }

        private string _changeableText = "this is a changeable text example that should serve as a testing ground for trying to see what happens when you change the text while the scrolling animation is playing";
        [UIValue("changeable-text")]
        public string ChangeableText
        {
            get => _changeableText;
            set
            {
                _changeableText = value;
                NotifyPropertyChanged();
            }
        }
        private bool _alwaysScroll = false;
        [UIValue("always-scroll")]
        public bool AlwaysScroll
        {
            get => _alwaysScroll;
            set
            {
                _alwaysScroll = value;
                NotifyPropertyChanged();
            }
        }
        private float _pauseDuration = 4f;
        [UIValue("pause-duration")]
        public float PauseDuration
        {
            get => _pauseDuration;
            set
            {
                _pauseDuration = value;
                NotifyPropertyChanged();
            }
        }
        private ScrollingText.ScrollAnimationType _animationType = ScrollingText.ScrollAnimationType.ForwardAndReverse;
        [UIValue("animation-type")]
        public ScrollingText.ScrollAnimationType AnimationType
        {
            get => _animationType;
            set
            {
                _animationType = value;
                NotifyPropertyChanged();
            }
        }
        private float _scrollSpeed = 15f;
        [UIValue("scroll-speed")]
        public float ScrollSpeed
        {
            get => _scrollSpeed;
            set
            {
                _scrollSpeed = value;
                NotifyPropertyChanged();
            }
        }


        [UIAction("click")]
        private void ButtonPress()
        {
            HeaderText = "It works!";
            buttonText.text = "Clicked";
            SomeNumber = 100234234;
        }

        [UIAction("cell click")]
        private void CellClick(TableView tableView, TestListObject testObj)
        {
            Logger.log.Info("Clicked - " + testObj.title);
        }

        [UIAction("keyboard-enter")]
        private void KeyboardEnter(string value)
        {
            Logger.log.Info("Keyboard typed: " + value);
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            List<CustomCellInfo> test = new List<CustomCellInfo>();
            for (int i = 0; i < 10; i++)
                test.Add(new CustomCellInfo("test" + i, "yee haw"));

            tableData.data = test;
            tableData.tableView.ReloadData();
        }

        [UIAction("text-1-button-clicked")]
        private void ScrollingTextText1ButtonClicked()
        {
            AlwaysScroll = false;
            ChangeableText = "short text example, stop animation";
        }

        [UIAction("text-2-button-clicked")]
        private void ScrollingTextText2ButtonClicked()
        {
            AlwaysScroll = false;
            ChangeableText = "this is a long text example that should cause the scrolling animation to play again. if it doesn't start the scrolling animation again, then something is wrong with the code";
        }

        [UIAction("text-3-button-clicked")]
        private void ScrollingTextText3ButtonClicked()
        {
            AlwaysScroll = true;
            ChangeableText = "short always scroll text example";
        }

        [UIAction("pause-duration-button-clicked")]
        private void ScrollingTextPauseDurationButtonClicked()
        {
            if (PauseDuration < 4f)
                PauseDuration = 4f;
            else
                PauseDuration = 1f;
        }

        [UIAction("animation-type-button-clicked")]
        private void ScrollingTextAnimationTypeButtonClicked()
        {
            if (AnimationType == ScrollingText.ScrollAnimationType.ForwardAndReverse)
                AnimationType = ScrollingText.ScrollAnimationType.Continuous;
            else
                AnimationType = ScrollingText.ScrollAnimationType.ForwardAndReverse;
        }

        [UIAction("scroll-speed-button-clicked")]
        private void ScrollingTextScrollSpeedButtonClicked()
        {
            if (ScrollSpeed < 15f)
                ScrollSpeed = 15f;
            else
                ScrollSpeed = 5f;
        }
    }
    public class TestListObject
    {
        [UIValue("title")]
        public string title;
        [UIValue("should-glow")]
        public bool shouldGlow;
        public TestListObject(string title, bool shouldGlow)
        {
            this.title = title;
            this.shouldGlow = shouldGlow;
        }

        [UIAction("button-click")]
        void ClickedButton()
        {
            Logger.log.Info("Button - " + title);
        }
    }
}
