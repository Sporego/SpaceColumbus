using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupMoveBrain : MonoBehaviour
{
    public static class Formation
    {
        public struct FormationParams
        {
            public FormationType formationType;
            public List<Vector3> orientations;
            public List<Vector3> positions;

            public static FormationParams NewFormation()
            {
                FormationParams formationParams;
                formationParams.formationType = FormationType.Line;
                formationParams.orientations = new List<Vector3>();
                formationParams.positions = new List<Vector3>();
                return formationParams;
            }
        }

        public enum FormationType : byte
        {
            Line,
            SingleLine,
        }

        // visualize with projectors
        public static FormationParams getFormationPositions(int numMembers, Vector3 p1, Vector3 p2, FormationType formationType)
        {
            FormationParams formationParams = FormationParams.NewFormation();

            // copy formation type
            formationParams.formationType = formationType;

            // compute the main 2d orientation of the formation
            Vector3 orientation = p2 - p1;
            orientation.y = 0;
            orientation = orientation.normalized;
            orientation = new Vector3(orientation.y, 0, -orientation.x);

            return formationParams;
        }

    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
