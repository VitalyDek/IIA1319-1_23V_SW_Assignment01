//Created by Vitaly Dekhtyarev 01.02.2022.

using System;
using System.Collections.Generic;
using System.Text;

interface ISen
{
    //An interface for creating new sensors. It is assumed that the followng properties are critical for all sensors.
    public string ID { get; set; }
    public string SenType { get; }
    public string Pos { get; set; }
    public string Unit { get; set; }
    public string Desc { get; set; }
}
interface ISenAnalog
{
    //An interface that is used to create sensors, which can measure continuous signal.
    public double Value { get; }
}
interface ISenDigit
{
    //An interface that is used to create sensors, which have a digital sensing component.
    public bool Value { get; }
}
class SensorA : ISen, ISenAnalog, IDisposable
{
    //A class for analog sensor. Includes all attributes and properties required by the interfaces.
    //Additionally, sensor has attributes contorling measurement range and time of the last sample.
    //The class includes a public method getValue(), which is supposed to be invoked, when a 
    //sensor takes measurement. This process may be time consuming and it was decided to
    //separate property returning the last measurement from the measuring process.
    //Type of the sensor is necessary to distinguish sensors.
    private string id;
    private double value=-1;
    private string txtPos;
    private string unit;
    private string desc;
    private DateTime timeSample;
    private bool disposed = false;
    private double valueMin;
    private double valueMax;
    private string senType = "A";

    public SensorA()
    {
        this.id = "-999";
        this.value = -999.99;
        this.txtPos = "Unknown";
        this.unit = "Unknown";
        this.desc = "Unknown";
        this.valueMin = 0;
        this.valueMax = 0;
    }
    //In windows form application all empty strings will be saved.
    public SensorA(string ID, string position, string unit, string desc, double min, double max)
    {
        this.id = ID;
        this.txtPos = position;
        this.unit = unit;
        this.desc = desc;
        //Maximum range limit cannot be lower than the minimum limit.
        if (max < min)
        {
            max = min;
        }
        this.valueMin = min;
        this.valueMax = max;
        this.value = min;
    }

    public string ID
    {
        get => this.id;
        set => this.id = value;
    }
    public string SenType
    {
        get => this.senType;
    }
    public string Pos
    {
        get => this.txtPos;
        set => this.txtPos = value;
    }
    public double Value
    {
        get => this.value;
    }
    public DateTime TimeSample
    {
        get => this.timeSample;
    }
    public string Unit
    {
        get => this.unit;
        set => this.unit = value;
    }
    public string Desc
    {
        get => this.desc;
        set => this.desc = value;
    }
    public void Dispose()
    {
        lock (this)
        {
            if (!this.disposed)
            {
                this.id = "-999";
                this.txtPos = null;
            }
            this.disposed = true;
            GC.SuppressFinalize(this);
        }
    }
    ~SensorA()
    {
        this.Dispose();
    }
    public string displayValues()
    {
        return $"Sensor: id:{this.id}, position:{this.txtPos}, value:{this.value}.";
    }
    public double getValue()
    {
        this.timeSample = DateTime.Now;
        this.value = valueCheck(genDblValue());
        return this.value;
    }
    private double valueCheck(double val)
    {
        if(val< this.valueMax && val > this.valueMin)
        {
            return val;
        }
        else
        {
            return double.NaN;
        }
    }
    private double genDblValue()
    {
        if(this.valueMin != this.valueMax)
        {
            return ((new Random()).NextDouble() * (this.valueMax - this.valueMin) + this.valueMin);
        }
        else
        {
            return this.valueMin;
        }
    }
}
class SensorD : ISen, ISenDigit, IDisposable
{
    //A digital sensor class. It is almost identical to the analog sensor class except that
    //no range limit is included.
    private string id;
    private bool value = false;
    private string txtPos;
    private string unit;
    private string desc;
    private DateTime timeSample;
    private bool disposed = false;
    private string senType = "D";

    public SensorD()
    {
        this.id = "-999";
        this.value = false;
        this.txtPos = "Unknown";
        this.unit = "Unknown";
        this.desc = "Unknown";
    }
    public SensorD(string ID, string position, string unit, string desc)
    {
        this.id = ID;
        this.txtPos = position;
        this.unit = unit;
        this.desc = desc;
        this.value = false;
    }
    public string ID
    {
        get => this.id;
        set => this.id = value;
    }
    public string SenType
    {
        get => this.senType;
    }
    public string Pos
    {
        get => this.txtPos;
        set => this.txtPos = value;
    }
    public bool Value
    {
        get => this.value;
    }
    public DateTime TimeSample
    {
        get => this.timeSample;
    }
    public string Unit
    {
        get => this.unit;
        set => this.unit = value;
    }
    public string Desc
    {
        get => this.desc;
        set => this.desc = value;
    }
    public void Dispose()
    {
        lock (this)
        {
            if (!this.disposed)
            {
                this.id = "-999";
                this.txtPos = null;
            }
            this.disposed = true;
            GC.SuppressFinalize(this);
        }
    }
    ~SensorD()
    {
        this.Dispose();
    }
    public string displayValues()
    {
        return $"Sensor: id:{this.id}, position:{this.txtPos}.";
    }
    public bool getValue()
    {
        this.timeSample = DateTime.Now;
        this.value = genFltValue();
        return this.value;
    }
    private bool genFltValue()
    {
        return (new Random().Next(0,2)==1) ? true : false;
    }
}