using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseGenerator<T, TParams>
{
    protected TParams Params;
    protected IEnumerable<IGeneratorCriteria<T>> GeneratorCriteria;

    protected BaseGenerator(TParams generatorParams, IEnumerable<IGeneratorCriteria<T>> generatorCriteria = null) {
        Params = generatorParams;
        GeneratorCriteria = generatorCriteria;
    }

    protected abstract T Generate();

    public T Execute()
    {
        T result = default; 
        try
        {
            result = Generate();

            if (result != null && GeneratorCriteria == null)
            {
                Debug.Log($"[Success] generation of: {typeof(T)}; attempts count: 1");
                return result;
            }
        } catch
        {
            result = default;
        }

        var attemptCount = 1;
        while ((result == null || (GeneratorCriteria != null && GeneratorCriteria.Any(_ => !_.Verify(result)))) && attemptCount <= GeneratorConstants.MaxGenerationAttempts)
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

        if (attemptCount > GeneratorConstants.MaxGenerationAttempts)
        {
            Debug.Log($"[Warning] Too many attempts to generate {GetType().ToString()}");
            return default;
        }

        Debug.Log($"[Success] generation of: {typeof(T)}; attempts count: {attemptCount}");

        return result;
    }
}

