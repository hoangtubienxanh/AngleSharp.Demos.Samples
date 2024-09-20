﻿namespace Samples.ViewModels
{
    using AngleSharp.Dom;
    using OxyPlot;
    using OxyPlot.Series;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class StatisticsViewModel : BaseViewModel, ITabViewModel
    {
        private PlotModel _mostElements;
        private PlotModel _mostClasses;
        private PlotModel _mostWords;
        private PlotModel _various;
        private PlotModel _mostAttributes;
        private IDocument _document;

        public PlotModel MostElements
        {
            get { return _mostElements; }
            set
            {
                _mostElements = value;
                RaisePropertyChanged();
            }
        }

        public PlotModel MostAttributes
        {
            get { return _mostAttributes; }
            set
            {
                _mostAttributes = value;
                RaisePropertyChanged();
            }
        }

        public PlotModel MostClasses
        {
            get { return _mostClasses; }
            set
            {
                _mostClasses = value;
                RaisePropertyChanged();
            }
        }

        public PlotModel MostWords
        {
            get { return _mostWords; }
            set
            {
                _mostWords = value;
                RaisePropertyChanged();
            }
        }

        public PlotModel Various
        {
            get { return _various; }
            set
            {
                _various = value;
                RaisePropertyChanged();
            }
        }

        public IDocument Document
        {
            get
            {
                return _document;
            }
            set
            {
                _document = value;
                var elements = new Dictionary<String, Int32>();
                var attributes = new Dictionary<String, Int32>();
                var classes = new Dictionary<String, Int32>();
                var words = new Dictionary<String, Int32>();
                var various = new Dictionary<String, Int32>();

                various.Add("Images", _document.Images.Length);
                various.Add("Scripts", _document.Scripts.Length);
                various.Add("Stylesheets", _document.StyleSheets.Length);
                various.Add("Plugins", _document.Plugins.Length);
                various.Add("Forms", _document.Forms.Length);

                Inspect(_document.DocumentElement, elements, classes, attributes);
                Words(_document.DocumentElement.TextContent.ToCharArray(), words);

                MostElements = CreatePieChart("Most elements", elements);
                MostClasses = CreatePieChart("Most classes", classes);
                MostWords = CreatePieChart("Most words", words);
                MostAttributes = CreatePieChart("Most attributes", attributes);
            }
        }

        private PlotModel CreatePieChart(String title, Dictionary<String, Int32> data)
        {
            var pm = new PlotModel();
            var ps = new PieSeries();
            pm.Title = title;
            pm.PlotMargins = new OxyThickness(0);
            pm.Padding = new OxyThickness(0);

            if (data.Count > 0)
            {
                var ranking = data.OrderByDescending(m => m.Value).Take(8);

                foreach (var element in ranking)
                {
                    ps.Slices.Add(new PieSlice(element.Key, element.Value));
                }
            }

            ps.InnerDiameter = 0.2;
            ps.ExplodedDistance = 0;
            ps.Stroke = OxyColors.White;
            ps.StrokeThickness = 1.0;
            ps.AngleSpan = 360;
            ps.StartAngle = 0;

            pm.Series.Add(ps);
            return pm;
        }

        private void Words(Char[] content, Dictionary<String, Int32> words)
        {
            var index = 0;
            var length = 0;

            for (var i = 0; i < content.Length; i++)
            {
                if (!Char.IsLetter(content[i]))
                {
                    length = i - index;

                    if (length > 1)
                    {
                        var word = new String(content, index, length);

                        if (words.ContainsKey(word))
                        {
                            words[word]++;
                        }
                        else
                        {
                            words.Add(word, 1);
                        }
                    }

                    index = i + 1;
                }
            }
        }

        private void Inspect(IElement element, Dictionary<String, Int32> elements, Dictionary<String, Int32> classes, Dictionary<String, Int32> attributes)
        {
            if (elements.ContainsKey(element.LocalName))
            {
                elements[element.LocalName]++;
            }
            else
            {
                elements.Add(element.LocalName, 1);
            }

            foreach (var cls in element.ClassList)
            {
                if (classes.ContainsKey(cls))
                {
                    classes[cls]++;
                }
                else
                {
                    classes.Add(cls, 1);
                }
            }

            foreach (var attr in element.Attributes)
            {
                if (attributes.ContainsKey(attr.Name))
                {
                    attributes[attr.Name]++;
                }
                else
                {
                    attributes.Add(attr.Name, 1);
                }
            }

            foreach (var child in element.Children)
            {
                Inspect(child, elements, classes, attributes);
            }
        }
    }
}
