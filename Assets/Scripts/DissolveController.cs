using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DissolveController : MonoBehaviour
{
    [SerializeField] private float dissolveSpeed = 0.25f;
    private readonly List<Material> mats = new();
    private float dissolveAmount = 0f;
    private bool isDissolving = false;

    private void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            mats.Add(renderers[i].material);
            mats.ElementAt(i).SetFloat("_DissolveAmount", 0f);
        }
    }

    private void Update()
    {
        if (!isDissolving) return;

        dissolveAmount += Time.deltaTime * dissolveSpeed;
        dissolveAmount = Mathf.Clamp01(dissolveAmount);
        foreach (Material mat in mats)
        {
            mat.SetFloat("_Dissolve_Amount", dissolveAmount);
        }

        if (dissolveAmount >= 1f)
        {
            isDissolving = false;
            Destroy(gameObject);
        }
    }

    public void StartDissolve()
    {
        isDissolving = true;
    }
}
