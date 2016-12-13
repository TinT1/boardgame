using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using CO = CoreObject;


public class Board//:IEnumerable<Field>
{
    
    public static int n = 9;

    public Field[,] fields = new Field[n, n];

    public Board()
    {
        for (int i = 0; i < Board.n; ++i)
            for (int j = 0; j < Board.n; ++j)
                fields[i, j] = new Field(i, j);
    }
    public static void Move(CoreObject coreObject, Field field)
    {
        if (field.type == Field.Type.Null) return;

        field.fieldObjects.Add(coreObject);
        coreObject.previousField = coreObject.currentField;
        coreObject.currentField = field;
        coreObject.previousField.fieldObjects.Remove(coreObject);
    }

    public static void Place(CoreObject coreObject, Field field)
    {
        field.fieldObjects.Add(coreObject);
        coreObject.previousField = coreObject.currentField;
        coreObject.currentField = field;
    }


  
    public Field this[int i,int j]
    {
        
        get { return fields[i, j]; }
        set { fields[i, j] = value; }
    }
    //*
    public static bool Contains(CO co)
    {
        return co.currentField!=null && co.currentField.fieldObjects.Contains(co);
    }

    public static void Remove(CoreObject coreObject)
    {
        coreObject.currentField.fieldObjects.Remove(coreObject);
    }
    public static void Kill(CoreObject target)
    {
        target.currentField.fieldObjects.RemoveAll(co => co.type == CoreObject.Type.Character);
    }
    
    private List<Field> FreeFields()
    {
        List<Field> freeFields = new List<Field>();

        for (int i = 0; i < fields.GetLength(0); ++i)
            for (int j = 0; j < fields.GetLength(1); ++j)
                freeFields.Add(fields[i,j]);

        return freeFields.FindAll(field => field.type == Field.Type.Black && field.fieldObjects.Count==0);
    }

    private Field RandomField()
    {
        List<Field> freeField = FreeFields();
        System.Random rand = new System.Random();
        int randIndex = UnityEngine.Random.Range(0, freeField.Count - 1); //rand.Next(0, freeField.Count - 1);
        return freeField[randIndex];
    }

    public bool TryRandomPlace(CoreObject coreObject)
    {
        if (this.FreeFields().Count == 0) return false;

        Place(coreObject, RandomField());
        return true;
    }
/*
    public bool isOnBoard(Field f) { return f.type != Field.Type.Null;  }


    public Field rightField(Field f) { if (isOnBoard(new Field(f.i + 1, f.j))) return this[f.i + 1, f.j]; else return f; }
    public Field leftField(Field f) { if (isOnBoard(new Field(f.i - 1, f.j))) return this[f.i - 1, f.j]; else return f; }
    public Field upField(Field f) { if (isOnBoard(new Field(f.i, f.j + 1))) return this[f.i, f.j + 1]; else return f; }
    public Field downField(Field f) { if (isOnBoard(new Field(f.i, f.j - 1))) return this[f.i, f.j - 1]; else return f; }

    public Field rightUpField(Field f) { if (isOnBoard(new Field(f.i + 1, f.j + 1))) return this[f.i + 1, f.j + 1]; else return f; }
    public Field leftUpField(Field f) { if (isOnBoard(new Field(f.i - 1, f.j + 1))) return this[f.i - 1, f.j + 1]; else return f; }
    public Field rightDownField(Field f) { if (isOnBoard(new Field(f.i + 1, f.j - 1))) return this[f.i + 1, f.j - 1]; else return f; }
    public Field leftDownField(Field f) { if (isOnBoard(new Field(f.i - 1, f.j - 1))) return this[f.i - 1, f.j - 1]; else return f; }
    */

}
