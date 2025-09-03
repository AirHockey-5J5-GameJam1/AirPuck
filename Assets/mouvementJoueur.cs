using UnityEngine;
using UnityEngine.InputSystem; // Use the new Input System namespace

public class mouvementJoueur : MonoBehaviour
{
    private GameObject selectedObject;
    Vector3 offset;

    void Update()
    {
        // Use Mouse.current instead of Input.mousePosition
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0f; // Make sure Z is 0 for 2D

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);

            if (targetObject)
            {
                selectedObject = targetObject.transform.gameObject;
                offset = selectedObject.transform.position - mousePosition;
            }
        }

        if (selectedObject)
        {
            selectedObject.transform.position = mousePosition + offset;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && selectedObject)
        {
            selectedObject = null;
        }
    }
}
