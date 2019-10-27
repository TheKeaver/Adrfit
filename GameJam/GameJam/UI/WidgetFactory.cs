﻿using System;
using System.Collections.Generic;
using Events;
using GameJam.UI.Widgets;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using UI.Content.Pipeline;

namespace GameJam.UI
{
    public static class WidgetFactory
    {
        public static Widget CreateFromPrototype(ContentManager content, WidgetPrototype prototype, ref Dictionary<string, WeakReference<Widget>> widgetIdDict)
        {
            Widget widget = null;

            AbstractValue width = ValueFromString(prototype.Width);
            AbstractValue height = ValueFromString(prototype.Height);

            AbstractValue horizontal = ValueFromString(prototype.Horizontal);
            AbstractValue vertical = ValueFromString(prototype.Vertical);

            HorizontalAlignment halign;
            switch (prototype.Halign.ToLower()[0])
            {
                case 'r':
                    halign = HorizontalAlignment.Right;
                    break;
                case 'l':
                    halign = HorizontalAlignment.Left;
                    break;
                case 'c':
                default:
                    halign = HorizontalAlignment.Center;
                    break;
            }
            VerticalAlignment valign;
            switch (prototype.Valign.ToLower()[0])
            {
                case 't':
                    valign = VerticalAlignment.Top;
                    break;
                case 'b':
                    valign = VerticalAlignment.Bottom;
                    break;
                case 'c':
                default:
                    valign = VerticalAlignment.Center;
                    break;
            }

            /** WIDGET SPECIALIZATIONS **/
            if (prototype is LabelWidgetPrototype)
            {
                BitmapFont font = content.Load<BitmapFont>(CVars.Get<string>(((LabelWidgetPrototype)prototype).Font));
                string labelContent = ((LabelWidgetPrototype)prototype).Content;

                widget = new Label(font, labelContent, halign, horizontal, valign, vertical, width, height);
            }
            if(prototype is ImageWidgetPrototype)
            {
                Texture2D texture = content.Load<Texture2D>(CVars.Get<string>(((ImageWidgetPrototype)prototype).Image));

                widget = new Image(texture, halign, horizontal, valign, vertical, width, height);
            }
            if (prototype is NinePatchImageWidgetPrototype)
            {
                Texture2D texture = content.Load<Texture2D>(CVars.Get<string>(((NinePatchImageWidgetPrototype)prototype).Image));

                string[] rawThickness = ((NinePatchImageWidgetPrototype)prototype).Thickness.Trim().Split(',');
                if(rawThickness.Length != 4)
                {
                    throw new Exception("NinePatchImage thickness must be integers in the for `left,top,right,bottom`.");
                }

                widget = new NinePatchImage(new NinePatchRegion2D(new TextureRegion2D(texture),
                        int.Parse(rawThickness[0]),
                        int.Parse(rawThickness[1]),
                        int.Parse(rawThickness[2]),
                        int.Parse(rawThickness[3])),
                    halign, horizontal, valign, vertical, width, height);
            }
            if (prototype is PanelWidgetPrototype)
            {
                widget = new Panel(halign, horizontal, valign, vertical, width, height);
            }
            if (prototype is ButtonWidgetPrototype)
            {
                Texture2D releasedTexture = content.Load<Texture2D>(CVars.Get<string>(((ButtonWidgetPrototype)prototype).ReleasedImage));
                string[] releasedRawThickness = ((ButtonWidgetPrototype)prototype).ReleasedThickness.Trim().Split(',');
                if (releasedRawThickness.Length != 4)
                {
                    throw new Exception("NinePatchImage thickness must be integers in the for `left,top,right,bottom`.");
                }

                Texture2D hoverTexture = content.Load<Texture2D>(CVars.Get<string>(((ButtonWidgetPrototype)prototype).HoverImage));
                string[] hoverRawThickness = ((ButtonWidgetPrototype)prototype).HoverThickness.Trim().Split(',');
                if (hoverRawThickness.Length != 4)
                {
                    throw new Exception("NinePatchImage thickness must be integers in the for `left,top,right,bottom`.");
                }

                Texture2D pressedTexture = content.Load<Texture2D>(CVars.Get<string>(((ButtonWidgetPrototype)prototype).PressedImage));
                string[] pressedRawThickness = ((ButtonWidgetPrototype)prototype).PressedThickness.Trim().Split(',');
                if (pressedRawThickness.Length != 4)
                {
                    throw new Exception("NinePatchImage thickness must be integers in the for `left,top,right,bottom`.");
                }

                widget = new Button(new NinePatchRegion2D(new TextureRegion2D(releasedTexture),
                        int.Parse(releasedRawThickness[0]),
                        int.Parse(releasedRawThickness[1]),
                        int.Parse(releasedRawThickness[2]),
                        int.Parse(releasedRawThickness[3])),
                    new NinePatchRegion2D(new TextureRegion2D(hoverTexture),
                        int.Parse(hoverRawThickness[0]),
                        int.Parse(hoverRawThickness[1]),
                        int.Parse(hoverRawThickness[2]),
                        int.Parse(hoverRawThickness[3])),
                    new NinePatchRegion2D(new TextureRegion2D(pressedTexture),
                        int.Parse(pressedRawThickness[0]),
                        int.Parse(pressedRawThickness[1]),
                        int.Parse(pressedRawThickness[2]),
                        int.Parse(pressedRawThickness[3])),
                    halign, horizontal, valign, vertical, width, height);
                if(((ButtonWidgetPrototype)prototype).OnClick.Length > 0) {
                    string[] onclickParts = ((ButtonWidgetPrototype)prototype).OnClick.Trim().Split(':');
                    switch(onclickParts[0].ToLower())
                    {
                        case "queue":
                            ((Button)widget).Action = () =>
                            {
                                string assemblyQualifiedName = string.Format("GameJam.Events.{0}, GameJam", onclickParts[1]);
                                IEvent evt = (IEvent)Activator.CreateInstance(Type.GetType(assemblyQualifiedName));
                                EventManager.Instance.QueueEvent(evt);
                            };
                            break;
                        default:
                            throw new Exception(string.Format("Unkown action type: `{0}`", onclickParts[0]));
                    }
                }
                ((Button)widget).rightID = ((ButtonWidgetPrototype)prototype).RightID;
                ((Button)widget).leftID = ((ButtonWidgetPrototype)prototype).LeftID;
                ((Button)widget).aboveID = ((ButtonWidgetPrototype)prototype).AboveID;
                ((Button)widget).belowID = ((ButtonWidgetPrototype)prototype).BelowID;
                ((Button)widget).isSelected = ((ButtonWidgetPrototype)prototype).IsSelected;
            }
            if (prototype is DropDownPanelWidgetPrototype)
            {
                Texture2D releasedTexture = content.Load<Texture2D>(CVars.Get<string>(((DropDownPanelWidgetPrototype)prototype).ReleasedImage));
                string[] releasedRawThickness = ((DropDownPanelWidgetPrototype)prototype).ReleasedThickness.Trim().Split(',');
                if (releasedRawThickness.Length != 4)
                {
                    throw new Exception("NinePatchImage thickness must be integers in the for `left,top,right,bottom`.");
                }

                Texture2D hoverTexture = content.Load<Texture2D>(CVars.Get<string>(((DropDownPanelWidgetPrototype)prototype).HoverImage));
                string[] hoverRawThickness = ((DropDownPanelWidgetPrototype)prototype).HoverThickness.Trim().Split(',');
                if (hoverRawThickness.Length != 4)
                {
                    throw new Exception("NinePatchImage thickness must be integers in the for `left,top,right,bottom`.");
                }

                Texture2D pressedTexture = content.Load<Texture2D>(CVars.Get<string>(((DropDownPanelWidgetPrototype)prototype).PressedImage));
                string[] pressedRawThickness = ((DropDownPanelWidgetPrototype)prototype).PressedThickness.Trim().Split(',');
                if (pressedRawThickness.Length != 4)
                {
                    throw new Exception("NinePatchImage thickness must be integers in the for `left,top,right,bottom`.");
                }

                AbstractValue contentsWidth = ValueFromString(((DropDownPanelWidgetPrototype)prototype).ContentsInfo.Width);
                AbstractValue contentsHeight = ValueFromString(((DropDownPanelWidgetPrototype)prototype).ContentsInfo.Height);

                string[] rawEventTypes = ((DropDownPanelWidgetPrototype)prototype).CloseOn.Split(',');
                Type[] eventTypes = new Type[rawEventTypes.Length];
                for(int i = 0; i < rawEventTypes.Length; i++)
                {
                    string rawEventType = rawEventTypes[i];
                    string assemblyQualifiedName = string.Format("GameJam.Events.{0}, GameJam", rawEventType);
                    eventTypes[i] = Type.GetType(assemblyQualifiedName);
                }

                widget = new DropDownPanel(new NinePatchRegion2D(new TextureRegion2D(releasedTexture),
                        int.Parse(releasedRawThickness[0]),
                        int.Parse(releasedRawThickness[1]),
                        int.Parse(releasedRawThickness[2]),
                        int.Parse(releasedRawThickness[3])),
                    new NinePatchRegion2D(new TextureRegion2D(hoverTexture),
                        int.Parse(hoverRawThickness[0]),
                        int.Parse(hoverRawThickness[1]),
                        int.Parse(hoverRawThickness[2]),
                        int.Parse(hoverRawThickness[3])),
                    new NinePatchRegion2D(new TextureRegion2D(pressedTexture),
                        int.Parse(pressedRawThickness[0]),
                        int.Parse(pressedRawThickness[1]),
                        int.Parse(pressedRawThickness[2]),
                        int.Parse(pressedRawThickness[3])),
                    halign, horizontal, valign, vertical, width, height,
                    contentsWidth, contentsHeight,
                    eventTypes);

                ((DropDownPanel)widget).rightID = ((DropDownPanelWidgetPrototype)prototype).RightID;
                ((DropDownPanel)widget).leftID = ((DropDownPanelWidgetPrototype)prototype).LeftID;
                ((DropDownPanel)widget).aboveID = ((DropDownPanelWidgetPrototype)prototype).AboveID;
                ((DropDownPanel)widget).belowID = ((DropDownPanelWidgetPrototype)prototype).BelowID;
                ((DropDownPanel)widget).isSelected = ((DropDownPanelWidgetPrototype)prototype).IsSelected;

                foreach (WidgetPrototype childPrototype in ((DropDownPanelWidgetPrototype)prototype).ContentsInfo.Children)
                {
                    ((DropDownPanel)widget).AddContent(CreateFromPrototype(content, childPrototype, ref widgetIdDict));
                }
            }

            if (widget is IParentWidget)
            {
                foreach (WidgetPrototype childPrototype in prototype.Children)
                {
                    ((IParentWidget)widget).Add(CreateFromPrototype(content, childPrototype, ref widgetIdDict));
                }
            }

            widget.Hidden = prototype.Hidden;
            widget.Alpha = prototype.Alpha;

            if (prototype.AspectRatio.Length > 0)
            {
                string[] aspectRatioParts = prototype.AspectRatio.Split(new[] { ':', '/' });
                float aspectRatio = 0;
                if (aspectRatioParts.Length == 1)
                {
                    aspectRatio = float.Parse(aspectRatioParts[0]);
                }
                if (aspectRatioParts.Length == 2)
                {
                    aspectRatio = float.Parse(aspectRatioParts[0]) / float.Parse(aspectRatioParts[1]);
                }
                widget.AspectRatio = aspectRatio;
                widget.MaintainAspectRatio = true;
            }

            if (prototype.ID.Trim().Length > 0)
            {
                if(widgetIdDict.ContainsKey(prototype.ID))
                {
                    throw new Exception(string.Format("Duplicate Widget ID: '{0}'", prototype.ID));
                }
                widgetIdDict.Add(prototype.ID, new WeakReference<Widget>(widget));
            }

            return widget;
        }

        private static AbstractValue ValueFromString(string s)
        {
            s = s.Trim();
            if(s[s.Length - 1] == '%')
            {
                // GetBaseValueFn will be filled in by the Widget on initialization
                return new RelativeValue(float.Parse(s.Substring(0, s.Length - 1)) / 100, null);
            }
            return new FixedValue(float.Parse(s));
        }
    }
}
