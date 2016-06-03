using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ToDos.Infrastructure.IoC
{
    /// <summary>
    ///     Instantiate this class in order to configure the framework.
    /// </summary>
    public abstract class Bootstrapper //: IDisposable
    {
        private readonly bool _isInitialized;
        private readonly Assembly _executingAssembly;


        /// <summary>
        ///     Creates an instance of the bootstrapper.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        protected Bootstrapper(Assembly executingAssembly)
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;

            _executingAssembly = executingAssembly;
            Start();
        }

        private void Start()
        {
            StartRuntime();
        }

        /// <summary>
        ///     Called by the bootstrapper's constructor at runtime to start the framework.
        /// </summary>
        protected virtual void StartRuntime()
        {
            Configure(_executingAssembly);
            OnStartup();
        }

        /// <summary>
        ///     Override to configure the framework and setup your IoC container.
        /// </summary>
        protected virtual void Configure(Assembly executingAssembly)
        {
        }

        /// <summary>
        ///     Override this to add custom behavior to execute after the application starts.
        /// </summary>
        protected virtual void OnStartup()
        {
        }

        /// <summary>
        ///     Override this to add custom behavior on exit.
        /// </summary>
        protected virtual void OnExit()
        {
        }
    }

    /// <summary>
    ///     A strongly-typed version of <see cref="Bootstrapper" /> that specifies the type of root model to create for the
    ///     application.
    /// </summary>
    /// <typeparam name="TRootModel">The type of root model for the application.</typeparam>
    public abstract class Bootstrapper<TRootModel> : Bootstrapper
    {
        protected Bootstrapper() : this(typeof (TRootModel).Assembly)
        {
        }

        protected Bootstrapper(Assembly executingAssembly) : base(executingAssembly)
        {
        }

        protected abstract T GetExportedValue<T>();

        /// <summary>
        ///     Override this to add custom behavior to execute after the application starts.
        /// </summary>
        protected override void OnStartup()
        {
            try
            {
                Root = GetExportedValue<TRootModel>();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null) throw;
                var loadEx = ex.InnerException as ReflectionTypeLoadException;
                if (loadEx == null) throw;
                var loaderExceptions = loadEx.LoaderExceptions;
                if (loaderExceptions == null) throw;
                foreach (var loaderException in loaderExceptions)
                {
                    throw loaderException;
                }
                throw;
            }
        }

        public TRootModel Root { get; private set; }
    }
}