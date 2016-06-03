﻿using System;
using ToDos.Infrastructure.Domain;

namespace ToDos.Infrastructure
{
    public class ToDosService : IToDosService
    {
        private readonly Func<IToDosContext> _toDosContextFactory;
        private IToDosContext _toDosContext;

        public ToDosService(Func<IToDosContext> toDosContextFactory)
        {
            _toDosContextFactory = toDosContextFactory;
        }

        public void Start()
        {
            _toDosContext = _toDosContextFactory();
        }

        public void Stop()
        {
            _toDosContext.Dispose();
            _toDosContext = null;
        }
    }
}