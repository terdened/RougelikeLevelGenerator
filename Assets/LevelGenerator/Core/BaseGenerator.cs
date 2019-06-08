using System.Collections.Generic;
using System.Linq;

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
        var result = Generate();

        if (_generatorCriteria == null)
            return result;

        var attemptCount = 1;
        while (_generatorCriteria.Any(_ => !_.Verify(result)) && attemptCount <= GeneratorConstants.MaxGenerationAttemts)
        {
            result = Generate();
            attemptCount++;
        }

        return result;
    }
}

