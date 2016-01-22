using UnityEngine;
using System.Collections;

public class CircleInterpreter : MonoBehaviour {

    public float diameter = 10f;
    public float samples = 30f;
    public int lineLength = 5;
    public bool fixedLength = false;
    public GameObject pixel;
    public GameObject line;
    public GameObject cameraObj;

    private int lineCounter = 0;
    private int ic = 0;

    private GameObject[] pixels;
    private float[] pixelUValues;
    private Vector2 previousPoint = new Vector2();

	// Use this for initialization
	void Start () {
        pixels = new GameObject[Mathf.RoundToInt(samples)];
        pixelUValues = new float[Mathf.RoundToInt(samples)];
        generateCircle();
	}
    //Creates the circle in pixel form
    public void generateCircle() {
        float inc = 2*Mathf.PI / samples;
        float currentPoint = 0;
        for(int j = 0; j < samples; j++) {
            Vector2 temp = getPoint(currentPoint);
            temp.x = Mathf.Round(temp.x);
            temp.y = Mathf.Round(temp.y);

            //In other words, a new point on the "display"
            if(temp.x!=previousPoint.x || temp.y != previousPoint.y) {
                GameObject pxl = (GameObject)Instantiate(pixel,new Vector3(temp.x,0,temp.y),Quaternion.identity);
                pixels[ic] = pxl;
                pixelUValues[ic] = currentPoint;
                ic++;
            }
            currentPoint += inc;
            previousPoint = temp;
        }
        //Now we can fix the camera position
        cameraObj.transform.position = new Vector3(diameter/2f,diameter,diameter/2f);
        if (fixedLength == false)
            approximateCircle();
        else
            approximateCircleFixed();
    }
    //IMPORTANT METHOD*********************************************************************
    //This is the method that traces the circle to a 1 pixel accuracy.
    public void approximateCircle() {
        bool hitEnd = false;
        int startNum = 0;
        int endNum = 1;

        Vector2 startPos = getPoint(pixelUValues[startNum]);
        Vector2 endPos = getPoint(pixelUValues[endNum]);

        while (hitEnd == false) {

            if (pixelUValues[endNum] == 0) {
                hitEnd = true;
                break;
            }

            for (int j = startNum; j < endNum; j++) {

                if (pointToLineDistance(startPos, endPos, getPoint(pixelUValues[j])) > 1f) {
                    endNum--;
                    endPos = getPoint(pixelUValues[endNum]);
                    GameObject linet = (GameObject)Instantiate(line, new Vector3((startPos.x + endPos.x) / 2f, 2f, (startPos.y + endPos.y) / 2f), Quaternion.identity);
                    linet.transform.localScale = new Vector3(Vector2.Distance(startPos, endPos), 1, 1);
                    linet.transform.rotation = Quaternion.FromToRotation(Vector3.right, new Vector3(endPos.x - startPos.x, 0, endPos.y - startPos.y));
                    startNum = endNum;
                    startPos = endPos;
                    lineCounter++;
                    break;
                }
            }
            endNum++;
            if (endNum >= pixels.Length) {
                hitEnd = true;
                break;
            }

            endPos = getPoint(pixelUValues[endNum]);
        }
        Debug.Log(lineCounter);
    }
    //Can be used to add a fixed number of lines to trace the circle
    public void approximateCircleFixed() {
        bool hitEnd = false;
        Vector2 startPos = new Vector2(pixels[0].transform.position.x, pixels[0].transform.position.z);
        Vector2 endPos = new Vector2(pixels[lineLength].transform.position.x, pixels[lineLength].transform.position.z);
       
        int endPosNum = lineLength;


        while (endPosNum < pixels.Length && hitEnd == false) {

            

            if (endPosNum+lineLength< pixelUValues.Length && pixelUValues[endPosNum + lineLength] != 0) {

                endPos = new Vector2(pixels[endPosNum].transform.position.x, pixels[endPosNum].transform.position.z);
                GameObject linet = (GameObject)Instantiate(line, new Vector3((startPos.x + endPos.x) / 2f, 2f, (startPos.y + endPos.y) / 2f), Quaternion.identity);
                linet.transform.localScale = new Vector3(Vector2.Distance(startPos, endPos), 1, 1);
                linet.transform.rotation = Quaternion.FromToRotation(Vector3.right, new Vector3(endPos.x - startPos.x, 0, endPos.y - startPos.y));
                startPos = endPos;
                endPosNum += lineLength;
                lineCounter++;
            } else {
                endPos = new Vector2(pixels[0].transform.position.x, pixels[0].transform.position.z);
                GameObject linet = (GameObject)Instantiate(line, new Vector3((startPos.x + endPos.x) / 2f, 2f, (startPos.y + endPos.y) / 2f), Quaternion.identity);
                linet.transform.localScale = new Vector3(Vector2.Distance(startPos,endPos), 1, 1);
                linet.transform.rotation = Quaternion.FromToRotation(Vector3.right, new Vector3(endPos.x - startPos.x, 0, endPos.y - startPos.y));
                hitEnd = true;
                lineCounter++;
            }

        }
        Debug.Log(lineCounter);
    }
    //Returns the actual point of the given u value
    public Vector2 getPoint(float u) {
        Vector2 p = new Vector2();
        p.x = Mathf.Sin(u) * diameter / 2f + (diameter/2f);
        p.y = Mathf.Cos(u) * diameter / 2f + (diameter/2f);

        return p;
    }
    //Returns the distance between a line and a point. Used to check that the distance is less than 1 pixel
    public float pointToLineDistance(Vector2 A, Vector2 B, Vector2 P) {
        float normalLength = Mathf.Sqrt((B.x - A.x) * (B.x - A.x) + (B.y - A.y) * (B.y - A.y));
        return Mathf.Abs((P.x - A.x) * (B.y - A.y) - (P.y - A.y) * (B.x - A.x)) / normalLength;
    }

}

