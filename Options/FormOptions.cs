﻿using SCJMapper_V2.Joystick;
using SCJMapper_V2.OGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SCJMapper_V2.Options
{
  public partial class FormOptions : Form
  {
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    private Label[] lblIn = null;
    private Label[] lblOut = null;
    private bool m_formLoaded = false;

    private BezierSeries m_bSeries = new BezierSeries( );

    private enum ESubItems
    {
      EControl = 0,
      EInverted,
      EExponent,
      EPt1,
      EPt2,
      EPt3,

      ESubItems_LAST
    }

    private OptionTree m_tuningRef = null; // will get the current optiontree on call
    public OptionTree OptionTree { get { return m_tuningRef; } set { m_tuningRef = value; } }

    private Deviceoptions m_devOptRef = null; // will get the current device options on call
    public Deviceoptions DeviceOptions { get { return m_devOptRef; } set { m_devOptRef = value; } }

    private DeviceList m_devListRef = null; // will get the current devices on call
    public DeviceList Devicelist { get { return m_devListRef; }
      set {
        m_devListRef = value;
      }
    }


    #region Form handling

    public FormOptions( )
    {
      InitializeComponent( );

      log.Info( "cTor - Entry" );

#if DEBUG
      btDebugStop.Visible = true;
#endif

      // helpers
      m_formLoaded = false;
      lblIn = new Label[] { null, lblLiveIn1, lblLiveIn2, lblLiveIn3, null, null };     // goes with PtNo 1..
      lblOut = new Label[] { null, lblLiveOut1, lblLiveOut2, lblLiveOut3, lblLiveOutExponent }; // goes with PtNo 1..

      // add 5 points to the chart data series ( Zero, user1..3, max)
      for ( int i = 0; i < 5; i++ ) {
        m_bSeries.BezierPoints.Add( new DataPoint( 0, 0 ) );
      }
      m_bSeries.ChartType = SeriesChartType.Line;
      m_bSeries.Name = "Curve";
      chart1.Series[0] = m_bSeries;
      // Create the Marker Series
      chart1.Series.Add( "Marker" );
      chart1.Series[1].ChartType = SeriesChartType.Point;
      chart1.Series[1].MarkerColor = Color.Orange;
      chart1.Series[1].MarkerStyle = MarkerStyle.Circle;
      chart1.Series[1].Points.AddXY( 0, 0 );
      chart1.Series[1].Points.AddXY( 0.25, 0.25 );
      chart1.Series[1].Points.AddXY( 0.5, 0.5 );
      chart1.Series[1].Points.AddXY( 0.75, 0.75 );
      chart1.Series[1].Points.AddXY( 1.0, 1.0 );

      log.Debug( "cTor - Exit" );
    }

    private void FormOptions_Load( object sender, EventArgs e )
    {
      log.Debug( "Load - Entry" );
      cobDevices.Items.Add( "Unassigned" ); // beware this makes the index of the combo one more than the Devlist !!
      if ( Devicelist != null ) {
        foreach (DeviceCls dev in Devicelist ) {
          cobDevices.Items.Add( dev.DevName );
        }
      }
      cobDevices.SelectedIndex = 0;

      OptionTreeSetup( );
      DevOptionsSetup( );

      PrepOptionsTab( );
      m_formLoaded = true;
      log.Debug( "Load - Exit" );
    }

    private void FormOptions_FormClosing( object sender, FormClosingEventArgs e )
    {
      log.Debug( "FormClosing - Entry" );
      UpdateLiveTuning( );
      log.Info( "Closed now" );
    }

    #endregion


    #region Setup

    private void OptionTreeSetup( )
    {
      log.Debug( "OptionTreeSetup - Entry" );
      if ( m_tuningRef == null ) {
        log.Error( "- OptionTreeSetup: m_tuningRef not assigned" );
        return;
      }

      lvOptionTree.Clear( );
      lvOptionTree.View = View.Details;
      lvOptionTree.LabelEdit = false;
      lvOptionTree.AllowColumnReorder = false;
      lvOptionTree.FullRowSelect = true;
      lvOptionTree.GridLines = true;
      lvOptionTree.CheckBoxes = false;
      lvOptionTree.MultiSelect = false;
      lvOptionTree.HideSelection = false;

      DeviceTuningParameter tuning = null;
      string option = "";
      ListViewGroup lvg;
      ListViewItem  lvi;

      lvg = new ListViewGroup( "0", "flight_move" ); lvOptionTree.Groups.Add( lvg );
      {
        option = "flight_move_pitch"; tuning = m_tuningRef.TuningItem( option ); m_live.Load( tuning );
        if ( m_live.used ) {
          lvi = new ListViewItem( option, lvg ); lvi.Name = option; lvi.Name = option; lvOptionTree.Items.Add( lvi );
          lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
          UpdateLvOptionFromLiveValues( m_live );
        }
        option = "flight_move_yaw"; tuning = m_tuningRef.TuningItem( option ); m_live.Load( tuning );
        if ( m_live.used ) {
          lvi = new ListViewItem( option, lvg ); lvi.Name = option; lvOptionTree.Items.Add( lvi );
          lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
          UpdateLvOptionFromLiveValues( m_live );
        }
        option = "flight_move_roll"; tuning = m_tuningRef.TuningItem( option ); m_live.Load( tuning );
        if ( m_live.used ) {
          lvi = new ListViewItem( option, lvg ); lvi.Name = option; lvOptionTree.Items.Add( lvi );
          lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
          UpdateLvOptionFromLiveValues( m_live );
        }
        option = "flight_move_strafe_vertical"; tuning = m_tuningRef.TuningItem( option ); m_live.Load( tuning );
        if ( m_live.used ) {
          lvi = new ListViewItem( option, lvg ); lvi.Name = option; lvOptionTree.Items.Add( lvi );
          lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
          UpdateLvOptionFromLiveValues( m_live );
        }
        option = "flight_move_strafe_lateral"; tuning = m_tuningRef.TuningItem( option ); m_live.Load( tuning );
        if ( m_live.used ) {
          lvi = new ListViewItem( option, lvg ); lvi.Name = option; lvOptionTree.Items.Add( lvi );
          lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
          UpdateLvOptionFromLiveValues( m_live );
        }
        option = "flight_move_strafe_longitudinal"; tuning = m_tuningRef.TuningItem( option ); m_live.Load( tuning );
        if ( m_live.used ) {
          lvi = new ListViewItem( option, lvg ); lvi.Name = option; lvOptionTree.Items.Add( lvi );
          lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
          UpdateLvOptionFromLiveValues( m_live );
        }
      }

      lvg = new ListViewGroup( "1", "flight_throttle" ); lvOptionTree.Groups.Add( lvg );
      {
        option = "flight_throttle_abs"; tuning = m_tuningRef.TuningItem( option ); m_live.Load( tuning );
        if ( m_live.used ) {
          lvi = new ListViewItem( option, lvg ); lvi.Name = option; lvOptionTree.Items.Add( lvi );
          lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
          UpdateLvOptionFromLiveValues( m_live );
        }
        option = "flight_throttle_rel"; tuning = m_tuningRef.TuningItem( option ); m_live.Load( tuning );
        if ( m_live.used ) {
          lvi = new ListViewItem( option, lvg ); lvi.Name = option; lvOptionTree.Items.Add( lvi );
          lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
          UpdateLvOptionFromLiveValues( m_live );
        }
      }

      lvg = new ListViewGroup( "2", "flight_aim" ); lvOptionTree.Groups.Add( lvg );
      {
        option = "flight_aim_pitch"; tuning = m_tuningRef.TuningItem( option ); m_live.Load( tuning );
        if ( m_live.used ) {
          lvi = new ListViewItem( option, lvg ); lvi.Name = option; lvOptionTree.Items.Add( lvi );
          lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
          UpdateLvOptionFromLiveValues( m_live );
        }
        option = "flight_aim_yaw"; tuning = m_tuningRef.TuningItem( option ); m_live.Load( tuning );
        if ( m_live.used ) {
          lvi = new ListViewItem( option, lvg ); lvi.Name = option; lvOptionTree.Items.Add( lvi );
          lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
          UpdateLvOptionFromLiveValues( m_live );
        }
      }

      lvg = new ListViewGroup( "3", "flight_view" ); lvOptionTree.Groups.Add( lvg );
      {
        option = "flight_view_pitch"; tuning = m_tuningRef.TuningItem( option ); m_live.Load( tuning );
        if ( m_live.used ) {
          lvi = new ListViewItem( option, lvg ); lvi.Name = option; lvOptionTree.Items.Add( lvi );
          lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
          UpdateLvOptionFromLiveValues( m_live );
        }
        option = "flight_view_yaw"; tuning = m_tuningRef.TuningItem( option ); m_live.Load( tuning );
        if ( m_live.used ) {
          lvi = new ListViewItem( option, lvg ); lvi.Name = option; lvOptionTree.Items.Add( lvi );
          lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
          UpdateLvOptionFromLiveValues( m_live );
        }
      }

      lvg = new ListViewGroup( "4", "Turret_aim" ); lvOptionTree.Groups.Add( lvg );
      {
        option = "turret_aim_pitch"; tuning = m_tuningRef.TuningItem( option ); m_live.Load( tuning );
        if ( m_live.used ) {
          lvi = new ListViewItem( option, lvg ); lvi.Name = option; lvOptionTree.Items.Add( lvi );
          lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
          UpdateLvOptionFromLiveValues( m_live );
        }
        option = "turret_aim_yaw"; tuning = m_tuningRef.TuningItem( option ); m_live.Load( tuning );
        if ( m_live.used ) {
          lvi = new ListViewItem( option, lvg ); lvi.Name = option; lvOptionTree.Items.Add( lvi );
          lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
          UpdateLvOptionFromLiveValues( m_live );
        }
      }


      lvOptionTree.Columns.Add( "Option", 180, HorizontalAlignment.Left );
      lvOptionTree.Columns.Add( "Dev Control", 80, HorizontalAlignment.Left );
      lvOptionTree.Columns.Add( "Invert", 50, HorizontalAlignment.Center );
      lvOptionTree.Columns.Add( "Expo.", 50, HorizontalAlignment.Center );
      lvOptionTree.Columns.Add( "Curve P1", 90, HorizontalAlignment.Center );
      lvOptionTree.Columns.Add( "Curve P2", 90, HorizontalAlignment.Center );
      lvOptionTree.Columns.Add( "Curve P3", 90, HorizontalAlignment.Center );

      lvOptionTree.ShowGroups = true;

      log.Debug( "OptionTreeSetup - Exit" );
    }

    private void DevOptionsSetup( )
    {
      log.Debug( "DevOptionsSetup - Entry" );
      if ( m_devOptRef == null ) {
        log.Error( "- DevOptionsSetup: m_devOptRef not assigned" );
        return;
      }

      lvDevOptions.Clear( );
      lvDevOptions.View = View.Details;
      lvDevOptions.LabelEdit = false;
      lvDevOptions.AllowColumnReorder = false;
      lvDevOptions.FullRowSelect = true;
      lvDevOptions.GridLines = true;
      lvDevOptions.CheckBoxes = false;
      lvDevOptions.MultiSelect = false;
      lvDevOptions.HideSelection = false;

      ListViewGroup lvg=null;
      ListViewItem  lvi;
      List<string> devNamesDone = new List<string>();

      foreach ( KeyValuePair<string, DeviceOptionParameter> kv in m_devOptRef ) {
        if ( !devNamesDone.Contains( kv.Value.DeviceName ) ) {
          lvg = new ListViewGroup( string.Format( "{0} - {1}", "jsN", kv.Value.DeviceName ) ); lvDevOptions.Groups.Add( lvg );
          devNamesDone.Add( kv.Value.DeviceName );
        }
        lvi = new ListViewItem( kv.Value.CommandCtrl, lvg ); lvi.Name = kv.Value.DoID; lvDevOptions.Items.Add( lvi );
        lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" ); lvi.SubItems.Add( "" );
        UpdateLvDevOptionFromLiveValues( kv.Value );
      }

      lvDevOptions.Columns.Add( "Dev Control", 40, HorizontalAlignment.Left );
      lvDevOptions.Columns.Add( "Action", 140, HorizontalAlignment.Left );
      lvDevOptions.Columns.Add( "Saturation", 80, HorizontalAlignment.Center );
      lvDevOptions.Columns.Add( "Deadzone", 80, HorizontalAlignment.Center );

      lvDevOptions.ShowGroups = true;

      log.Debug( "DevOptionsSetup - Exit" );
    }


    #endregion



    public void SetTuningRecord( DeviceTuningParameter tp )
    {

    }

    private void button2_Click( object sender, EventArgs e )
    {
      ; // break
    }

    #region class LiveValues (internal class)

    private class LiveValues
    {
      // load live from TuningParameters
      public void Load( DeviceTuningParameter dp )
      {
        optionName = dp.OptionName;

        if ( dp != null ) {
          Reset( );
          used = true; // always

          gameDeviceRef = dp.GameDevice;
          nodetext = dp.NodeText;
          if ( ! string.IsNullOrEmpty( dp.NodeText ) ) {
            string[] e = nodetext.Split( new char[] { ActionTreeInputNode.RegDiv, ActionTreeInputNode.ModDiv }, StringSplitOptions.RemoveEmptyEntries );
            if ( e.Length > 0 )
              control = e[1].TrimEnd( );
            else
              control = dp.NodeText;
          }else if ( gameDeviceRef != null ) {
            control = gameDeviceRef.DevName;
          }else { 
            control = "n.a.";
          }
          command = dp.CommandCtrl;
          // the option data
          if ( dp.Deviceoption != null ) {
            deadzoneUsed = dp.Deviceoption.DeadzoneUsed;
            deadzoneS = dp.Deviceoption.Deadzone;
            saturationUsed = dp.Deviceoption.SaturationUsed;
            saturationS = dp.Deviceoption.Saturation;
          } else {
            deadzoneUsed = false;
            saturationUsed = false;
          }

          invertUsed = dp.InvertUsed;
          exponentUsed = dp.ExponentUsed;
          exponentS = dp.Exponent;
          nonLinCurveUsed = dp.NonLinCurveUsed;
          if ( dp.NonLinCurveUsed ) {
            nonLinCurve.Curve( float.Parse( dp.NonLinCurvePtsIn[0] ), float.Parse( dp.NonLinCurvePtsOut[0] ),
                                   float.Parse( dp.NonLinCurvePtsIn[1] ), float.Parse( dp.NonLinCurvePtsOut[1] ),
                                   float.Parse( dp.NonLinCurvePtsIn[2] ), float.Parse( dp.NonLinCurvePtsOut[2] ) );
          } else {
            // dummy curve
            nonLinCurve.Curve( 0.25f, 0.25f, 0.5f, 0.5f, 0.75f, 0.75f );
          }
        }
      }

      // load live from DevOptions
      public void Load( DeviceOptionParameter dp )
      {
        optionName = dp.CommandCtrl;

        if ( dp != null ) {
          Reset( );
          used = true;

          nodetext = dp.CommandCtrl;
          control = dp.CommandCtrl;
          command = dp.CommandCtrl;
          // the option data
          deadzoneUsed = dp.DeadzoneUsed;
          deadzoneS = dp.Deadzone;
          saturationUsed = dp.SaturationUsed;
          saturationS = dp.Saturation;

          invertUsed = false;
          exponentUsed = false;
          nonLinCurveUsed = false;
        }
      }

      // update the TuningParameters
      public void Update( ref DeviceTuningParameter dp )
      {
        if ( !used ) return;
        // don't return strings to control the device
        if ( AcceptGameDevice ) {
          dp.GameDevice = gameDeviceRef;
        }

        dp.InvertUsed = invertUsed;
        if ( dp.Deviceoption != null ) {
          dp.Deviceoption.DeadzoneUsed = deadzoneUsed;
          dp.Deviceoption.Deadzone = deadzoneS;
          dp.Deviceoption.SaturationUsed = saturationUsed;
          dp.Deviceoption.Saturation = saturationS;
        }
        dp.ExponentUsed = exponentUsed;
        dp.Exponent = exponentS;
        dp.NonLinCurveUsed = nonLinCurveUsed;
        List<string> pts = new List<string>( );
        pts.Add( nonLinCurve.Pt( 0 ).X.ToString( "0.000" ) ); pts.Add( nonLinCurve.Pt( 1 ).X.ToString( "0.000" ) ); pts.Add( nonLinCurve.Pt( 2 ).X.ToString( "0.000" ) );
        dp.NonLinCurvePtsIn = pts;
        pts = new List<string>( );
        pts.Add( nonLinCurve.Pt( 0 ).Y.ToString( "0.000" ) ); pts.Add( nonLinCurve.Pt( 1 ).Y.ToString( "0.000" ) ); pts.Add( nonLinCurve.Pt( 2 ).Y.ToString( "0.000" ) );
        dp.NonLinCurvePtsOut = pts;
      }

      // update the TuningParameters
      public void Update( ref DeviceOptionParameter dp )
      {
        if ( !used ) return;
        // don't return strings to control the device
          dp.DeadzoneUsed = deadzoneUsed;
          dp.Deadzone = deadzoneS;
          dp.SaturationUsed = saturationUsed;
          dp.Saturation = saturationS;
      }

      public void Reset( )
      {
        used = false;
        nodetext = ""; control = ""; command = "";
        m_range = 1000.0; m_sign = 1.0;
        invertUsed = false;
        deadzoneUsed = false; deadzone = 0.0;
        saturationUsed = false; saturation = 1000.0;
        exponentUsed = false; exponent = 1.0;
        nonLinCurveUsed = false;
      }

      // Context
      public bool used = false;
      public string optionName = "";

      public string nodetext = "";  // the node text
      public string control = ""; // the device control item e.g.  js2_x
      public string command = ""; // the control item used to get the XDevice Input
      public string devOptCommand { get { return command.Replace( "throttle", "" ); } } // have to get rid of throttle for devOptions..
      public bool AcceptGameDevice { get { return string.IsNullOrEmpty( nodetext ); } } // this is how we do it..
      public DeviceCls gameDeviceRef = null;

      // calc values
      private const double MAX_DZ = 160.0; // avoid range issues and silly values..
      private const double MIN_SAT = 200.0; // avoid range issues and silly values..
      private double m_range = 1000.0;
      private double m_sign = 1.0;


      // set values
      public bool m_invertUsed = false;
      public bool invertUsed { get { return m_invertUsed; } set { m_invertUsed = value; m_sign = m_invertUsed ? -1.0 : 1.0; } }
      public string invertS
      {
        get {
          if ( invertUsed )
            return "yes";
          else
            return "no";
        }
        set {
          if ( value == "yes" )
            m_invertUsed = true;
          else
            m_invertUsed = false;

          m_sign = m_invertUsed ? -1.0 : 1.0;
        }
      }

      public bool deadzoneUsed = false;
      private double m_deadzone = 0.0; // stores 1000 * set value
      public double deadzone { get { return m_deadzone; } set { m_deadzone = ( value > MAX_DZ ) ? MAX_DZ : value; m_range = m_saturation - m_deadzone; } }
      public string deadzoneS // get/set game value 0..1.000
      {
        get { return ( deadzone / 1000.0 ).ToString( "0.000" ); }
        set {
          double f;
          if ( double.TryParse( value, out f ) ) {
            deadzone = f * 1000.0;
          } else {
            deadzone = 0.0;
          }
        }
      }

      public bool saturationUsed = false;
      private double m_saturation = 1000.0;// stores 1000 * set value
      public double saturation { get { return m_saturation; } set { m_saturation = ( value < MIN_SAT ) ? MIN_SAT : value; m_range = m_saturation - m_deadzone; } }
      public string saturationS // get/set game value 0..1.000
      {
        get { return ( m_saturation / 1000.0 ).ToString( "0.000" ); }
        set {
          double f;
          if ( double.TryParse( value, out f ) ) {
            saturation = f * 1000.0;
          } else {
            saturation = 1000.0;
          }
        }
      }

      public bool exponentUsed = false;
      public double exponent = 1.0F;
      public string exponentS
      {
        get {
          if ( exponentUsed )
            return exponent.ToString( "0.000" );
          else
            return "1.000";
        }
        set {
          double f;
          if ( double.TryParse( value, out f ) ) {
            exponent = f;
          } else {
            exponent = 1.0;
          }
        }
      }

      public bool nonLinCurveUsed = false;
      public xyPoints nonLinCurve = new xyPoints( 1000 );  // max val of Joystick Input

      /// <summary>
      /// returns the Point string value
      /// </summary>
      /// <param name="ptIndex">The IN point (1..3)</param>
      /// <returns>A formatted string</returns>
      public string PtInS( int ptIndex )
      {
        if ( !nonLinCurveUsed )
          return ( 0.25 * ptIndex ).ToString( "0.000" );
        else
          return nonLinCurve.Pt( ptIndex - 1 ).X.ToString( "0.000" );
      }
      /// <summary>
      /// returns the Point string value
      /// </summary>
      /// <param name="ptIndex">The OUT point (1..3)</param>
      /// <returns>A formatted string</returns>
      public string PtOutS( int ptIndex )
      {
        if ( !nonLinCurveUsed )
          return ( 0.25 * ptIndex ).ToString( "0.000" );
        else
          return nonLinCurve.Pt( ptIndex - 1 ).Y.ToString( "0.000" );
      }
      /// <summary>
      /// returns the Point string value pair
      /// </summary>
      /// <param name="ptIndex">The IN / OUT point (1..3)</param>
      /// <returns>A formatted string</returns>
      public string PtS( int ptIndex )
      {
        if ( !nonLinCurveUsed )
          return ( 0.25 * ptIndex ).ToString( "0.000" ) + " / " + ( 0.25 * ptIndex ).ToString( "0.000" );
        else
          return nonLinCurve.Pt( ptIndex - 1 ).X.ToString( "0.000" ) + " / " + nonLinCurve.Pt( ptIndex - 1 ).Y.ToString( "0.000" );
      }
    } // class LiveValues

    #endregion

    private LiveValues m_live = new LiveValues();
    private DeviceTuningParameter m_liveTuning = null;
    private DeviceOptionParameter m_liveDevOption = null;


    // update the GUI from Live
    private void UpdateGUIFromLiveValues( LiveValues lv )
    {
      log.Debug( "UpdateGUIFromLiveValues - Entry" );
      // guard input mess handling...
      m_updatingPts++;

      if ( !lv.used ) {
        // reset
        pnlOptionInput.Enabled = false;
        lblLiveNodetext.Text = "---";
        cbxLiveInvert.Checked = false;
        lblLiveOutDeadzone.Text = "0.000"; cbxUseDeadzone.Checked = false; tbDeadzone.Enabled = false;
        lblLiveOutSaturation.Text = "1.000"; cbxUseSaturation.Checked = false; tbSaturation.Enabled = false;
        lblLiveOutExponent.Text = "1.000"; rbLivePtExponent.Checked = false;
        lblLiveIn1.Text = "0.250"; lblLiveOut1.Text = "0.250"; lblLiveIn2.Text = "0.500"; lblLiveOut2.Text = "0.500"; lblLiveIn3.Text = "0.750"; lblLiveOut3.Text = "0.750";
        rbLivePt1.Checked = false; rbLivePt2.Checked = false; rbLivePt3.Checked = false;
        rbUseNone.Checked = true;
        cbxLiveInvert.Enabled = false;

        m_updatingPts--; // end guard
        log.Debug( "UpdateGUIFromLiveValues - Exit 'not used'" );
        return;
      }
      // get values from Live storage
      pnlOptionInput.Enabled = true;
      lblLiveNodetext.Text = lv.nodetext;

      pnlDevOptionInput.Visible = ! lv.AcceptGameDevice; // cannot assign DevOptions to Tuning parameters without Action (will just dumped the Option only)
      cobDevices.Enabled = lv.AcceptGameDevice;
      if ( m_live.gameDeviceRef != null ) {
        int idx = cobDevices.Items.IndexOf( m_live.gameDeviceRef.DevName );
        if ( idx >= 0 )
          cobDevices.SelectedIndex = idx;
      }
      else {
        cobDevices.SelectedIndex = 0; // unassigned
      }

      if ( lv.deadzoneUsed ) lblLiveOutDeadzone.Text = lv.deadzoneS;
      cbxUseDeadzone.Checked = lv.deadzoneUsed;
      if ( lv.saturationUsed ) lblLiveOutSaturation.Text = lv.saturationS;
      cbxUseSaturation.Checked = lv.saturationUsed;

      cbxLiveInvert.Enabled = true;
      cbxLiveInvert.Checked = lv.invertUsed;

      rbUseNone.Checked = true; // init - we will see later if it changes (guarded - so no sideeffects from Checked Events)
      if ( lv.exponentUsed ) lblLiveOutExponent.Text = lv.exponentS;
      rbUseExpo.Checked = lv.exponentUsed;  // Update to used one - if so..
      rbLivePtExponent.Checked = lv.exponentUsed;

      if ( lv.nonLinCurveUsed ) {
        lblLiveIn1.Text = lv.nonLinCurve.Pt( 0 ).X.ToString( "0.000" ); lblLiveOut1.Text = lv.nonLinCurve.Pt( 0 ).Y.ToString( "0.000" );
        lblLiveIn2.Text = lv.nonLinCurve.Pt( 1 ).X.ToString( "0.000" ); lblLiveOut2.Text = lv.nonLinCurve.Pt( 1 ).Y.ToString( "0.000" );
        lblLiveIn3.Text = lv.nonLinCurve.Pt( 2 ).X.ToString( "0.000" ); lblLiveOut3.Text = lv.nonLinCurve.Pt( 2 ).Y.ToString( "0.000" );
      }
      rbUsePts.Checked = lv.nonLinCurveUsed; // Update to used one - if so..
      rbLivePt1.Checked = lv.nonLinCurveUsed; rbLivePt2.Checked = false; rbLivePt3.Checked = false; // mark Pt1 to start with

      m_updatingPts--; // end guard

      log.Debug( "UpdateGUIFromLiveValues - Exit" );
    }


    private void UpdateLvOptionFromLiveValues( LiveValues lv )
    {
      log.Debug( "UpdateLvOptionFromLiveValues - Entry" );
      if ( !lvOptionTree.Items.ContainsKey( lv.optionName ) ) {
        log.Error( "ERROR: UpdateLvOptionFromLiveValues - Did not found Option: " + lv.optionName );
        log.Debug( "UpdateLvOptionFromLiveValues - Exit 'not used'" );
        return;
      }

      ListViewItem lvi = lvOptionTree.Items[lv.optionName];
      if ( !lv.used ) {
        // leave alone.. for next time enabling it
        lvi.SubItems[1].Text = m_live.control; // js4_x
        lvi.SubItems[2].Text = "---"; lvi.SubItems[3].Text = "---"; // inverted .. expo
        lvi.SubItems[4].Text = "--- / ---"; lvi.SubItems[5].Text = "--- / ---"; lvi.SubItems[6].Text = "--- / ---"; // pt1..3
      } else {
        lvi.SubItems[1].Text = m_live.control; // js4_x
        lvi.SubItems[2].Text = m_live.invertS;
        if ( m_live.exponentUsed )
          lvi.SubItems[3].Text = m_live.exponentS; // inverted .. expo
        else
          lvi.SubItems[3].Text = "---"; // inverted .. expo
        if ( m_live.nonLinCurveUsed ) {
          lvi.SubItems[4].Text = m_live.PtS( 1 ); lvi.SubItems[5].Text = m_live.PtS( 2 ); lvi.SubItems[6].Text = m_live.PtS( 3 ); // pt1..3
        } else {
          lvi.SubItems[4].Text = "--- / ---"; lvi.SubItems[5].Text = "--- / ---"; lvi.SubItems[6].Text = "--- / ---"; // pt1..3
        }
      }

      log.Debug( "UpdateLvOptionFromLiveValues - Exit" );
    }


    private void UpdateLvDevOptionFromLiveValues( DeviceOptionParameter dp )
    {
      log.Debug( "UpdateLvDevOptionFromLiveValues - Entry" );
      if ( !lvDevOptions.Items.ContainsKey( dp.DoID ) ) {
        log.Error( "ERROR: UpdateLvDevOptionFromLiveValues - Did not found Option: " + dp.DoID );
        log.Debug( "UpdateLvDevOptionFromLiveValues - Exit 'not used'" );
        return;
      }

      ListViewItem lvi = lvDevOptions.Items[dp.DoID];
      lvi.SubItems[1].Text = dp.Action; // Action 
      if ( dp.SaturationUsed )
        lvi.SubItems[2].Text = dp.Saturation; // saturation
      else
        lvi.SubItems[2].Text = "---";
      if ( dp.DeadzoneUsed )
        lvi.SubItems[3].Text = dp.Deadzone; // deadzone
      else
        lvi.SubItems[3].Text = "---";

      log.Debug( "UpdateLvDevOptionFromLiveValues - Exit" );
    }


    #region Charts section

    // Chart - move Pts

    /// <summary>
    /// Evaluate which tune parameter has the chart input
    /// </summary>
    private void EvalChartInput( )
    {
      log.Debug( "EvalChartInput - Entry" );

      m_hitPt = 0;
      if ( rbUsePts.Checked && rbLivePt1.Enabled && rbLivePt1.Checked ) m_hitPt = 1;
      if ( rbUsePts.Checked && rbLivePt2.Enabled && rbLivePt2.Checked ) m_hitPt = 2;
      if ( rbUsePts.Checked && rbLivePt3.Enabled && rbLivePt3.Checked ) m_hitPt = 3;
      if ( rbUseExpo.Checked && rbLivePtExponent.Enabled && rbLivePtExponent.Checked ) m_hitPt = 4;

      //      if ( m_hitPt > 0 ) return;

      // slider fudge
      tbDeadzone.Enabled = false;
      if ( cbxUseDeadzone.Enabled && cbxUseDeadzone.Checked ) {
        tbDeadzone.Enabled = true;
        tbDeadzone.Value = ( int )( float.Parse( lblLiveOutDeadzone.Text ) * 250f );
      }

      tbSaturation.Enabled = false;
      if ( cbxUseSaturation.Enabled && cbxUseSaturation.Checked ) {
        tbSaturation.Enabled = true;
        tbSaturation.Value = ( int )( ( float.Parse( lblLiveOutSaturation.Text ) - 0.2f ) * 50f );
      }
      EvalSlider( );

      log.Debug( "EvalChartInput - Exit" );
    }


    private int m_updatingPts = 0;
    /// <summary>
    /// Handle change of the mouse input within the chart
    /// </summary>
    private void rbPtAny_CheckedChanged( object sender, EventArgs e )
    {
      if ( m_updatingPts > 0 ) return;
      // start guard
      m_updatingPts++;

      //Depending on the selected radio button change the inputs..

      // prev state here
      if ( m_live.exponentUsed ) {
        if ( rbUseExpo.Checked ) {
          ; // just nothing
        } else if ( rbUsePts.Checked ) {
          // switch to NonLin
          rbLivePtExponent.Enabled = false; rbLivePtExponent.Checked = false;
          m_live.exponentUsed = false;
          rbLivePt1.Enabled = true; rbLivePt2.Enabled = true; rbLivePt3.Enabled = true;
          rbLivePt1.Checked = true; // start with the first one
          m_live.nonLinCurveUsed = true;
        } else {
          //Switch to none
          rbLivePtExponent.Enabled = false; rbLivePtExponent.Checked = false;
          m_live.exponentUsed = false;
        }

      } else if ( m_live.nonLinCurveUsed ) {
        if ( rbUseExpo.Checked ) {
          // switch to expo
          rbLivePt1.Enabled = false; rbLivePt2.Enabled = false; rbLivePt3.Enabled = false;
          rbLivePt1.Checked = false; rbLivePt2.Checked = false; rbLivePt3.Checked = false;
          m_live.nonLinCurveUsed = false;
          rbLivePtExponent.Enabled = true; rbLivePtExponent.Checked = true;
          m_live.exponentUsed = true;
        } else if ( rbUsePts.Checked ) {
          ; // just nothing
        } else {
          //Switch to none
          rbLivePt1.Enabled = false; rbLivePt2.Enabled = false; rbLivePt3.Enabled = false;
          rbLivePt1.Checked = false; rbLivePt2.Checked = false; rbLivePt3.Checked = false;
          m_live.nonLinCurveUsed = false;
        }

      } else {
        // prev was None
        if ( rbUseExpo.Checked ) {
          // switch to expo
          rbLivePtExponent.Enabled = true; rbLivePtExponent.Checked = true;
          m_live.exponentUsed = true;
        } else if ( rbUsePts.Checked ) {
          // switch to NonLin
          rbLivePt1.Enabled = true; rbLivePt2.Enabled = true; rbLivePt3.Enabled = true;
          rbLivePt1.Checked = true; // start with the first one
          m_live.nonLinCurveUsed = true;
        } else {
          //Switch to none
          ; // just nothing
        }
      }

      // EvalChartInput( );

      UpdateChartItems( );
      // end guard
      m_updatingPts--;
    }


    // handle mouse interaction with the chart

    int m_hitPt = 0;
    bool m_hitActive = false;
    int mX = 0; int mY = 0;

    /// <summary>
    /// Update the graph from changes of acitve label values
    /// </summary>
    private void UpdateChartItems( )
    {
      log.Debug( "UpdateChartItems - Entry" );

      bool deadzoneUsed = true;
      bool satUsed = true;
      bool expUsed = true;
      bool ptsUsed = true;
      // see what is on display..
      // Yaw
      deadzoneUsed = ( m_live.deadzoneUsed == true );
      satUsed = ( m_live.saturationUsed == true );
      expUsed = ( m_live.exponentUsed == true );
      ptsUsed = ( m_live.nonLinCurveUsed == true );
      lblGraphDeadzone.Text = m_live.deadzoneS;
      lblGraphSaturation.Text = m_live.saturationS;

      // generic part
      lblGraphDeadzone.Visible = deadzoneUsed;

      lblGraphSaturation.Visible = satUsed;


      rbLivePtExponent.Enabled = expUsed;
      rbLivePt1.Enabled = ptsUsed; rbLivePt2.Enabled = ptsUsed; rbLivePt3.Enabled = ptsUsed;
      EvalChartInput( );  // review active chart input

      if ( !tbDeadzone.Enabled ) lblLiveOutDeadzone.Text = "0.000";
      if ( !tbSaturation.Enabled ) lblLiveOutSaturation.Text = "1.000";

      if ( expUsed ) {
        // Exp mode
        double expo = double.Parse( lblOut[4].Text );
        // dont touch zero Point
        m_bSeries.BezierPoints[1].SetValueXY( 0.25, Math.Pow( 0.25, expo ) );
        m_bSeries.BezierPoints[2].SetValueXY( 0.5, Math.Pow( 0.5, expo ) );
        m_bSeries.BezierPoints[3].SetValueXY( 0.75, Math.Pow( 0.75, expo ) );
        m_bSeries.BezierPoints[4].SetValueXY( 1.0, 1.0 );

      } else if ( ptsUsed ) {
        // Pts mode
        // dont touch zero Point
        for ( int i = 1; i <= 3; i++ ) {
          m_bSeries.BezierPoints[i].SetValueXY( float.Parse( lblIn[i].Text ), float.Parse( lblOut[i].Text ) );
        }
        m_bSeries.BezierPoints[4].SetValueXY( 1.0, 1.0 );

      } else {
        // linear
        // dont touch zero Point
        m_bSeries.BezierPoints[1].SetValueXY( 0.25, 0.25 );
        m_bSeries.BezierPoints[2].SetValueXY( 0.5, 0.5 );
        m_bSeries.BezierPoints[3].SetValueXY( 0.75, 0.75 );
        m_bSeries.BezierPoints[4].SetValueXY( 1.0, 1.0 );
      }
      // update markers from curve points
      chart1.Series[1].Points[1] = m_bSeries.BezierPoints[1];
      chart1.Series[1].Points[2] = m_bSeries.BezierPoints[2];
      chart1.Series[1].Points[3] = m_bSeries.BezierPoints[3];
      chart1.Series[1].Points[4] = m_bSeries.BezierPoints[4];

      m_bSeries.Invalidate( chart1 );

      log.Debug( "UpdateChartItems - Exit" );
    }



    private void chartPoint_MouseDown( object sender, System.Windows.Forms.MouseEventArgs e )
    {
      m_hitActive = true; // activate movement tracking
      mX = e.X; mY = e.Y; // save initial loc to get deltas
    }

    private void chartPoint_MouseMove( object sender, System.Windows.Forms.MouseEventArgs e )
    {
      if ( m_hitActive ) {
        if ( m_hitPt < 1 ) {
          // nothing selected ...
        } else if ( m_hitPt <= 3 ) {
          // Pt1..3
          double newX = double.Parse( lblIn[m_hitPt].Text ) + ( e.X - mX ) * 0.001f; mX = e.X;
          newX = ( newX > 1.0f ) ? 1.0f : newX;
          newX = ( newX < 0.0f ) ? 0.0f : newX;
          lblIn[m_hitPt].Text = newX.ToString( "0.000" );

          double newY = double.Parse( lblOut[m_hitPt].Text ) + ( e.Y - mY ) * -0.001f; mY = e.Y;
          newY = ( newY > 1.0f ) ? 1.0f : newY;
          newY = ( newY < 0.0f ) ? 0.0f : newY;
          lblOut[m_hitPt].Text = newY.ToString( "0.000" );

          // update chart (Points[0] is zero)
          m_bSeries.BezierPoints[m_hitPt].SetValueXY( newX, newY );
          // update markers from curve points
          chart1.Series[1].Points[m_hitPt] = m_bSeries.BezierPoints[m_hitPt];

        } else if ( m_hitPt == 4 ) {
          // Exponent
          double newY = double.Parse( lblOut[m_hitPt].Text ) + ( e.Y - mY ) * 0.01f; mY = e.Y;
          newY = ( newY > 3.0f ) ? 3.0f : newY;
          newY = ( newY < 0.5f ) ? 0.5f : newY;
          lblOut[m_hitPt].Text = newY.ToString( "0.000" );

          // update chart (Points[0] is zero)
          m_bSeries.BezierPoints[1].SetValueXY( 0.25, Math.Pow( 0.25, newY ) );
          m_bSeries.BezierPoints[2].SetValueXY( 0.5, Math.Pow( 0.5, newY ) );
          m_bSeries.BezierPoints[3].SetValueXY( 0.75, Math.Pow( 0.75, newY ) );
        }

        // update markers from curve points
        chart1.Series[1].Points[1] = m_bSeries.BezierPoints[1];
        chart1.Series[1].Points[2] = m_bSeries.BezierPoints[2];
        chart1.Series[1].Points[3] = m_bSeries.BezierPoints[3];
        chart1.Series[1].Points[4] = m_bSeries.BezierPoints[4];

        m_bSeries.Invalidate( chart1 );

      }
    }

    private void chartPoint_MouseUp( object sender, System.Windows.Forms.MouseEventArgs e )
    {
      m_hitActive = false;

      // update live values
      m_live.exponentS = lblLiveOutExponent.Text;
      if ( m_live.nonLinCurve != null ) {
        m_live.nonLinCurve.Curve( float.Parse( lblLiveIn1.Text ), float.Parse( lblLiveOut1.Text ),
                              float.Parse( lblLiveIn2.Text ), float.Parse( lblLiveOut2.Text ),
                              float.Parse( lblLiveIn3.Text ), float.Parse( lblLiveOut3.Text ) );
      }
    }
    #endregion


    #region Slider Value Changed (Deadzone / Saturation)

    // Deadzone slider   00 .. 40 -> 0 .. 0.160 ( 4 pt scale)
    // Saturation slider 00 .. 40 -> 0.2 .. 1.0 ( 20 pt scale)

    /// <summary>
    /// Update Live from Slider Value
    /// </summary>
    private void EvalSlider( )
    {
      lblLiveOutDeadzone.Text = ( tbDeadzone.Value / 250.0f ).ToString( "0.000" );
      if ( cbxUseDeadzone.Enabled && cbxUseDeadzone.Checked ) {
        float curDeadzone = 1000.0f * ( tbDeadzone.Value / 250.0f );  // % scaled to maxAxis
        m_live.deadzone = curDeadzone;
        lblGraphDeadzone.Text = lblLiveOutDeadzone.Text;

      }
      lblLiveOutSaturation.Text = ( tbSaturation.Value / 50.0f + 0.200f ).ToString( "0.000" );
      if ( cbxUseSaturation.Enabled && cbxUseSaturation.Checked ) {
        float curSaturation = 1000.0f * ( tbSaturation.Value / 50.0f + 0.2f);  // % scaled to maxAxis
        m_live.saturation = curSaturation;
        lblGraphSaturation.Text = lblLiveOutSaturation.Text;
      }
    }
    #endregion

    #region Checked Invert Changed

    private void cbxInvert_CheckedChanged( object sender, EventArgs e )
    {
      m_live.invertUsed = false;
      if ( cbxLiveInvert.Checked == true ) {
        m_live.invertUsed = true;
      }
    }

    #endregion

    #region Checked Deadzone Changed

    private void cbxUseDeadzone_CheckedChanged( object sender, EventArgs e )
    {
      m_live.deadzoneUsed = false;
      if ( cbxUseDeadzone.Checked == true ) {
        tbDeadzone.Enabled = true;
        m_live.deadzoneUsed = true;
      } else {
        tbDeadzone.Enabled = false;
        tbDeadzone.Value = tbDeadzone.Minimum;
      }
      EvalChartInput( );
      UpdateGUIFromLiveValues( m_live );
    }

    #endregion

    #region Checked Saturation Changed

    private void cbxUseSaturation_CheckedChanged( object sender, EventArgs e )
    {
      m_live.saturationUsed = false;
      if ( cbxUseSaturation.Checked == true ) {
        tbSaturation.Enabled = true;
        m_live.saturationUsed = true;
      } else {
        tbSaturation.Enabled = false;
        tbSaturation.Value = tbSaturation.Maximum;
      }
      EvalChartInput( );
      UpdateGUIFromLiveValues( m_live );
    }

    #endregion



    private void tbSlider_ValueChanged( object sender, EventArgs e )
    {
      EvalSlider( );
    }

    /// <summary>
    /// Update the last Tuning item and the ListView Item from Live
    /// </summary>
    private void UpdateLiveTuning( )
    {
      if ( m_liveTuning != null ) {
        m_live.Update( ref m_liveTuning );
        UpdateLvOptionFromLiveValues( m_live );
        m_liveTuning = null;
      }
    }

    private void UpdateLiveDevOption( )
    {
      if ( m_liveDevOption != null ) {
        m_live.Update( ref m_liveDevOption );
        UpdateLvDevOptionFromLiveValues( m_liveDevOption );
        m_liveDevOption = null;
      }
    }

    private void PrepOptionsTab( )
    {
      pnlOptionInput.Visible = true;
      if ( lvOptionTree.Items.Count > 0 )
        lvOptionTree.Items[0].Selected = true;
    }

    private void PrepDevOptionsTab( )
    {
      pnlOptionInput.Visible = false;
      if ( lvDevOptions.Items.Count > 0 )
        lvDevOptions.Items[0].Selected = true;
    }


    // get the Live Item updated
    private void lvOptionTree_SelectedIndexChanged( object sender, EventArgs e )
    {
      log.Debug( "lvOptionTree_SelectedIndexChanged - Entry" );
      try {
        if ( ( sender as ListView ).SelectedItems.Count > 0 ) {
          // before loading a new one we push the current one back to tuning and the list view
          UpdateLiveTuning( );
          ListViewItem lvi = ( sender as ListView ).SelectedItems[0];
          m_liveTuning = m_tuningRef.TuningItem( lvi.Name );
          m_live.Load( m_liveTuning );
          UpdateGUIFromLiveValues( m_live );

          UpdateChartItems( );
        }
      } catch {
        ;
      }
      log.Debug( "lvOptionTree_SelectedIndexChanged - Exit" );
    }


    private void lvDevOptions_SelectedIndexChanged( object sender, EventArgs e )
    {
      log.Debug( "lvDevOptions_SelectedIndexChanged - Entry" );
      try {
        if ( ( sender as ListView ).SelectedItems.Count > 0 ) {
          // before loading a new one we push the current one back to tuning and the list view
          UpdateLiveDevOption( );
          ListViewItem lvi = ( sender as ListView ).SelectedItems[0];
          m_liveDevOption = m_devOptRef[lvi.Name];
          m_live.Load( m_liveDevOption );
          UpdateGUIFromLiveValues( m_live );

          UpdateChartItems( );
        }
      } catch {
        ;
      }
      log.Debug( "lvDevOptions_SelectedIndexChanged - Exit" );
    }


    private void tabC_Selecting( object sender, TabControlCancelEventArgs e )
    {
      log.Debug( "tabC_Selecting - Entry" );
      if ( ( e.TabPageIndex == 0 ) && ( e.Action == TabControlAction.Deselecting ) ) {
        // before leaving the Tab we push the current one back to tuning and the list view
        UpdateLiveTuning( );
        m_liveTuning = null;
        m_live.Reset( );
      } else
      if ( ( e.TabPageIndex == 0 ) && ( e.Action == TabControlAction.Selecting ) ) {
        PrepOptionsTab( );
      } else
      if ( ( e.TabPageIndex == 1 ) && ( e.Action == TabControlAction.Deselecting ) ) {
        UpdateLiveDevOption( );
        m_liveDevOption = null;
        m_live.Reset( );
      } else
      if ( ( e.TabPageIndex == 1 ) && ( e.Action == TabControlAction.Selecting ) ) {
        PrepDevOptionsTab( );
      } else { }

      e.Cancel = false; // let it change
      log.Debug( "tabC_Selecting - Exit" );
    }

    private void btExit_Click( object sender, EventArgs e )
    {
      // It ai setup as OK button - nothing here so far...
    }

    private void cobDevices_SelectedIndexChanged( object sender, EventArgs e )
    {

      if ( m_live.AcceptGameDevice ) {
        if ( cobDevices.SelectedIndex <= 0 ) {
          m_live.gameDeviceRef = null;
          m_live.control = "n.a.";
        } else {
          m_live.gameDeviceRef = m_devListRef[cobDevices.SelectedIndex-1]; // we have the empty element on top in the combo
          m_live.control = m_live.gameDeviceRef.DevName;
        }
      }
    }


  }
}
