using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class DissolveController : MonoBehaviour
{
    [SerializeField] private float dissolveSpeed = 1f;
    public List<Material> mats;
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

        StartCoroutine(StartDissolveCoroutine());
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
    }

    public void StartDissolve()
    {
        isDissolving = true;
    }

    private IEnumerator StartDissolveCoroutine()
    {
        yield return new WaitForSeconds(10f);

        StartDissolve();
    }
}
