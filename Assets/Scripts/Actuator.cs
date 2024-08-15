using Oculus.Interaction.Input;
using UnityEngine;

public class Actuator : MonoBehaviour {

    public HalfEdge a;
    public HalfEdge b;

    
    [Range(0, 1)] public float throttle = 1.0f;
    public float pressure = 1.0f;

    public float pressureLag = 2.0f;


    public void Start() {

    }

    public void Update() {
        if (currentController) {
            float controllerInput = currentController.ControllerInput.Primary2DAxis.y;
            throttle = Mathf.Clamp01(throttle + Time.deltaTime * controllerInput);
        }

        pressure = Mathf.Lerp(throttle, pressure, Mathf.Exp(- Time.deltaTime / pressureLag));
        a.AddPressureForce(pressure);
        b.AddPressureForce(pressure);

        GetComponent<AudioSource>().pitch = 2 * throttle;
        GetComponent<AudioSource>().volume = throttle;
    }


    private Controller currentController = null;
    public void EnableInput(Controller c) {
        currentController = c;
    }

    public void DisableInput() {
        currentController = null;
    }
}