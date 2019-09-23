using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseGenerator<T, TParams>
{
    protected TParams _params;
    protected IEnumerable<IGeneratorCriteria<T>> _generatorCriteria;

    public BaseGenerator(TParams generatorParams, IEnumerable<IGeneratorCriteria<T>> generatorCriteria = null) {
        _params = generatorParams;
        _generatorCriteria = generatorCriteria;
    }

    protected abstract T Generate();

    public T Execute()
    {
        T result = default; 
        try
        {
            result = Generate();

            if (result != null && _generatorCriteria == null)
            {
                Debug.Log($"[Success] generation of: {typeof(T)}; attempts count: 1");
                return result;
            }
        } catch
        {
            result = default;
        }

        var attemptCount = 1;
        while ((result == null || (_generatorCriteria != null && _generatorCriteria.Any(_ => !_.Verify(result)))) && attemptCount <= GeneratorConstants.MaxGenerationAttemts)
        {
            try
            {
                result = Generate();
            }
            catch(Exception e)
            {
                result = default;
            }
            finally {
                attemptCount++;
            }
        }

        if (attemptCount > GeneratorConstants.MaxGenerationAttemts)
        {
            Debug.Log($"[Warning] Too many attempts to generate {GetType().ToString()}");
            return default;
        }

        Debug.Log($"[Success] generation of: {typeof(T)}; attempts count: {attemptCount}");

        return result;
    }
}

