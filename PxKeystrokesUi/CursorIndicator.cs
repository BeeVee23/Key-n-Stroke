﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PxKeystrokesWPF;

namespace PxKeystrokesUi
{
    public partial class CursorIndicator : Form
    {
        IMouseRawEventProvider m;
        SettingsStore s;

        public CursorIndicator(IMouseRawEventProvider m, SettingsStore s)
        {
            InitializeComponent();

            this.m = m;
            this.s = s;
            FormClosed += CursorIndicator_FormClosed;

            NativeMethodsWindow.SetWindowTopMost(this.Handle);
            SetFormStyles();

            m.MouseEvent += m_MouseEvent;
            Paint += CursorIndicator_Paint;
            s.PropertyChanged += settingChanged;

            BackColor = Color.Lavender;
            TransparencyKey = Color.Lavender;
        }

        void CursorIndicator_Paint(object sender, PaintEventArgs e)
        {
            this.Location = new Point(0, 0);
            Graphics g = this.CreateGraphics();
            Pen p = new Pen(s.CursorIndicatorColor, 7);
            g.FillEllipse(p.Brush, 0, 0, (int) s.CursorIndicatorSize, (int) s.CursorIndicatorSize);
            //UpdatePosition();
        }

        Point cursorPosition;

        void m_MouseEvent(MouseRawEventArgs raw_e)
        {
            cursorPosition = raw_e.Position;
            if(raw_e.Action == MouseAction.Move)
                UpdatePosition();
        }

        void CursorIndicator_FormClosed(object sender, FormClosedEventArgs e)
        {
            if ( m != null )
                m.MouseEvent -= m_MouseEvent;
            if ( s != null )
                s.PropertyChanged -= settingChanged;
            m = null;
            s = null;
        }

        void SetFormStyles()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Opacity = s.CursorIndicatorOpacity;
            NativeMethodsGWL.ClickThrough(this.Handle);
            NativeMethodsGWL.HideFromAltTab(this.Handle);

            UpdateSize();
            UpdatePosition();
        }

        

        void UpdateSize()
        {
            this.Size = new Size((int)s.CursorIndicatorSize, (int)s.CursorIndicatorSize);
            this.Invalidate(new Rectangle(0, 0, this.Size.Width, this.Size.Height));
        }

        void UpdatePosition()
        {
            this.Location = Point.Subtract(cursorPosition, new Size(this.Size.Width / 2, this.Size.Height / 2));
            //this.Location = cursorPosition;
        }

        private void settingChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "EnableCursorIndicator":
                    break;
                case "CursorIndicatorOpacity":
                    this.Opacity = s.CursorIndicatorOpacity;
                    break;
                case "CursorIndicatorSize":
                    UpdateSize();
                    break;
                case "CursorIndicatorColor":
                    UpdateSize(); // invalidates
                    break;
            }
        }


    }
}
