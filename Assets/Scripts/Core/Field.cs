using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Field
{
    public enum Type { Black,White,Null}


    public Coordinates coordinates;
    public Type type;
    public List<CoreObject> fieldObjects = new List<CoreObject>();

    public Field(int i,int j)
    {
        this.coordinates = new Coordinates(i, j);

        bool whiteBool = i == 0 && (j == 0 || j == Board.n / 2 || j == Board.n - 1)
                    || i == Board.n / 2 && (j == 0 || j == Board.n - 1)
                    || i == Board.n - 1 && (j == 0 || j == Board.n / 2 || j == Board.n - 1);
        bool nullBool = i == 0 || j ==0 || i == Board.n - 1  || j == Board.n - 1;

        type =      whiteBool ? Type.White : ( nullBool ? Type.Null : Type.Black);
    }

   
    public static int CoordinateDistance(Field field1, Field field2)	{	return Coordinates.Distance(field1.coordinates, field2.coordinates);	}
    public static bool SameCoordinates(Field field1, Field field2)		{	return Coordinates.IsEqual(field1.coordinates, field2.coordinates);		}

    
    public string Print()												{	return coordinates.Print();												}


    public GUISkin Skin()												{	return skins[type];														}
    public bool Show() 													{ 	return type != Type.Null; 												}
	
    public static void SetSkin()
    {
        skins[Type.Black] = Resources.Load("Skins/Field/Black") as GUISkin;
        skins[Type.White] = Resources.Load("Skins/Field/White") as GUISkin;
        skins[Type.Null] = Resources.Load("Skins/Field/Null") as GUISkin;
    }
    public static Dictionary<Type, GUISkin> skins = new Dictionary<Type, GUISkin>();

}

public partial class Field
{
    public class Coordinates
    {
        public int i,j;

	    public Coordinates(int i,int j)											{	this.i = i;			this.j = j;										}
        public Coordinates(Coordinates coor)									{	this.i = coor.i;	this.j = coor.j;								}
	
		public static bool IsEqual(Coordinates cor1, Coordinates cor2)			{	return  cor1.i == cor2.i && cor2.j == cor1.j;						}
        public static Coordinates operator +(Coordinates c1, Coordinates c2)	{	return new Coordinates(c1.i + c2.i, c1.j + c2.j);					}
        public static Coordinates operator -(Coordinates c1, Coordinates c2)	{	return new Coordinates(c1.i - c2.i, c1.j - c2.j);					}
        public static int Distance(Coordinates c1, Coordinates c2)				{	return Mathf.Max(Mathf.Abs(c1.i - c2.i), Mathf.Abs(c1.j - c2.j));	}
	
        public string Print()													{	return "" + i + " " + j;											}
    }
}
