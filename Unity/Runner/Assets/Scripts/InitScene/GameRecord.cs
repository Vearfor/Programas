using TauriLand.Libreria;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

//----------------------------------------------------------------------
// Registro para cada partida
//----------------------------------------------------------------------
// - guardar tiempo
// - numero de items
// Tiene que ser public para que la lista sea publica
// (que lo mismo no hace falta)
//----------------------------------------------------------------------
public class GameRecord
{
    //----------------------------------------------------------------------
    // Propiedades
    //----------------------------------------------------------------------
    public DateTime when { get; set; }
    public int itemsReached { get; set; }
    public float secondsOfPlay { get; set; }
    public float distance { get; set; }
    //----------------------------------------------------------------------

    //----------------------------------------------------------------------
    // Constructor
    //----------------------------------------------------------------------
    public GameRecord()
    {
        reset();
    }
    //----------------------------------------------------------------------

    //----------------------------------------------------------------------
    public static GameRecord parser(string line, int cont = 0)
    {
        GameRecord rec = null;

        //
        // Tool.LogColor(" " + cont + " [" + line + "]", Color.aliceBlue);
        // # inicio de comentario
        // # > inicio de linea valida
        // # ; separador de campos
        // # sera >fecha+hora;items;segundos;
        //

        if (line.StartsWith("#"))
            return rec;

        if (line.StartsWith(">"))
        {
            rec = new GameRecord();

            // Quitamos el '>' del principio:
            string sLine = line.Substring(1);
            string[] parts = sLine.Split(";");

            //--------------------------------------------------------------
            try
            {
                rec.when = DateTime.Parse(parts[0]);
            }
            catch (Exception)
            {
                rec.when = DateTime.Now;
            }
            //--------------------------------------------------------------
            try
            {
                rec.distance = float.Parse(parts[1]);
            }
            catch (Exception)
            {
                rec.distance = 0f;
            }
            //--------------------------------------------------------------
            try
            {
                rec.itemsReached = int.Parse(parts[2]);
            }
            catch (Exception)
            {
                rec.itemsReached = 0;
            }
            //--------------------------------------------------------------
            try
            {
                rec.secondsOfPlay = float.Parse(parts[3]);
            }
            catch (Exception)
            {
                rec.secondsOfPlay = 0f;
            }
            //--------------------------------------------------------------
        }
        return rec;
    }

    //----------------------------------------------------------------------
    public static bool isBestRecord(GameRecord rec1, GameRecord rec2)
    {
        bool isBetter = false;

        if (rec1.itemsReached>rec2.itemsReached)
        {
            isBetter = true;
        }
        else
        {
            if (rec1.itemsReached == rec2.itemsReached)
            {
                if (rec1.distance > rec2.distance)
                {
                    isBetter = true;
                }
                else
                {
                    if (rec1.distance == rec2.distance)
                    {
                        if (rec1.secondsOfPlay < rec2.secondsOfPlay)
                        {
                            isBetter = true;
                        }
                        else
                        {
                            if (rec1.when < rec2.when)
                            {
                                isBetter = true;
                            }
                        }
                    }
                }
            }
        }

        return isBetter;
    }

    //----------------------------------------------------------------------
    // Devuelve de los dos el que sea el mejor.
    //----------------------------------------------------------------------
    public static GameRecord getBestRecord(GameRecord rec1, GameRecord rec2)
    {
        GameRecord best = null;

        if (GameRecord.isBestRecord(rec1, rec2))
        {
            best = rec1;
        }
        else
        {
            best = rec2;
        }

        return best;
    }

    //----------------------------------------------------------------------
    // ToString Del GameRecord
    //----------------------------------------------------------------------
    public override string ToString()
    {
        string sValor = 
            string.Format(
                "Value: {0} items, {1:0.00} metros, en {2:0.00} segundos. [{3}]",
                itemsReached.ToString().PadLeft(2),
                distance.ToString().PadLeft(2),
                secondsOfPlay.ToString().PadLeft(5),
                when.ToString()
        );
        return sValor;
    }

    internal void reset()
    {
        when = DateTime.Now;
        itemsReached = 0;
        secondsOfPlay = 0f;
        distance = 0f;
    }
    //----------------------------------------------------------------------
}


public class GameRecordList : List<GameRecord>
{
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    string pathGameList;
    string fileGameList;
    //----------------------------------------------------------------------
    GameRecord bestRecord;
    //----------------------------------------------------------------------

    //----------------------------------------------------------------------
    // Constructor
    //----------------------------------------------------------------------
    public GameRecordList(string pathGameList, string filGameList)
    {
        this.pathGameList = pathGameList;
        this.fileGameList = filGameList;
        bestRecord = new GameRecord();
    }

    //----------------------------------------------------------------------
    // Crea una GameRecordList a partir de la lectura de un fichero
    //----------------------------------------------------------------------
    public static GameRecordList readingGameList(string pathFile, string fileList)
    {
        GameRecordList list = null;

        string fileToRead = ((!string.IsNullOrEmpty(pathFile)) ? (pathFile + "/") : "") + fileList;

        if (!File.Exists(fileToRead))
        {
            // No sera un error, puede que no hayamos jugado todavia
            Tool.LogColor("No hay fichero de partidas: [" + fileToRead + "] donde leer", Color.yellow);
            return list;
        }

        //------------------------------------------------------------------
        // Podemos leer el fichero de golpe, que no sera muy grande
        // pero lo vamos a hacer linea a linea
        // El using, creo recordar, se preocupaba con el StreamReder
        // de cerrar, en el destructor (o en el metodo 'dispose'),
        // el fichero.
        //------------------------------------------------------------------
        // La condicion para utilizar el StreamReader con using era que la
        // clase fuera del interfaz que fuerza a tener el metodo Dispose
        //------------------------------------------------------------------
        try
        {
            using (StreamReader sr = new StreamReader(fileToRead))
            {
                Tool.LogColor("Lectura de fichero de partidas", Color.green);

                list = new GameRecordList(pathFile, fileList);

                int cont = 0;
                for (string line = sr.ReadLine(); line != null; line = sr.ReadLine())
                {
                    GameRecord gameRecord = GameRecord.parser(line, cont++);
                    if (gameRecord != null)
                        list.Add(gameRecord);
                }
                Tool.LogColor("- leidos " + cont + " registros", Color.green);
                Tool.LogColor("- agregados " + list.Count + " registros", Color.green);
            }
        }
        catch (Exception ex)
        {
            Tool.LogColor("Exception: GameRecordList readingGameList: [" + ex.Message + "]", Color.red);
            list = null;
        }

        return list;
    }

    //----------------------------------------------------------------------
    // Crea un Fichero de GameRecord's a partir de un GameRecordList
    // Internamente el GameRecordList ya tiene el path y el fichero donde
    // escribir.
    //----------------------------------------------------------------------
    public static void writingGameList(GameRecordList list)
    {
        // Lo crearemos si tenemos registros:
        if (list == null || (list != null && list.Count == 0))
            return;

        string fileToWriting = ((!string.IsNullOrEmpty(list.pathGameList)) ? (list.pathGameList + "/") : "") + list.fileGameList;

        try
        {
            using (StreamWriter sr = new StreamWriter(fileToWriting))
            {
                Tool.LogColor("Escritura del fichero de partidas: " + list.Count + " registros", Color.green);

                string sLine =
                    string.Format("#\n{0}{1}{2}#",
                    "# para mostrar que empezar con # es comentario\n",
                    "# > inicio de linea de registro valida\n",
                    "# >fecha;items;segundos\n");
                sr.WriteLine(sLine);

                foreach (GameRecord record in list)
                {
                    sLine = string.Format(">{0};{1};{2};{3}",
                            record.when.ToString("yyyy/MM/dd HH:mm:ss"),
                            record.distance.ToString("0.00"),
                            record.itemsReached.ToString("00"),
                            record.secondsOfPlay.ToString("0.00")
                        );

                    sr.WriteLine(sLine);
                }
            }
        }
        catch (Exception ex)
        {
            Tool.LogColor("Exception: GameRecordList writingGameList: [" + ex.Message + "]", Color.red);
        }
    }

    //----------------------------------------------------------------------
    // Ordena la lista de partidas
    // - de la mas valiosa a la que menos.
    //   + mas objetivos en menos tiempo
    //----------------------------------------------------------------------
    public static void sortGameList(GameRecordList list)
    {
        // Lo crearemos si tenemos registros:
        if (list == null || (list != null && list.Count == 0))
            return;

        for (int i = 0; i < list.Count; i++)
        {
            GameRecord recordi = list[i];
            for (int j = i+1; j< list.Count; j++)
            {
                GameRecord recordj = list[j];
                if (GameRecord.isBestRecord(recordj, recordi))
                {
                    list[j] = recordi;
                    list[i] = recordj;
                    recordi = recordj;
                }
            }
        }

    }

    public static GameRecord getBestGameRecord(GameRecordList list)
    {
        GameRecord bestRecord;

        bestRecord = list.bestRecord;

        for (int i = 0; i< list.Count; i++)
        {
            bestRecord = GameRecord.getBestRecord(bestRecord, list[i]);
        }

        list.bestRecord = bestRecord;

        return bestRecord;
    }
}
