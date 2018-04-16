﻿using Build;

namespace UnitTests
{
    namespace TestSet4
    {
        public interface IPersonRepository
        {
            Person GetPerson(int personId);
        }
        public class Person
        {
            readonly IPersonRepository _personRepository;

            public Person(IPersonRepository personRepository)
            {
                _personRepository = personRepository;
            }
        }

        //[Dependency(typeof(SqlDataRepository), RuntimeInstance.Singleton)]
        public class SqlDataRepository : IPersonRepository
        {
            [DependencyAttribute("Ho ho ho")]
            public SqlDataRepository()
            {
            }

            [DependencyAttribute(typeof(SqlDataRepository), RuntimeInstance.Singleton)]
            public SqlDataRepository(int personId)
            {
            }

            public Person GetPerson(int personId)
            {
                // get the data from SQL DB and return Person instance.
                return new Person(this);
            }
        }

        public class ServiceDataRepository : IPersonRepository
        {
            public ServiceDataRepository([Injection(typeof(SqlDataRepository))]IPersonRepository repository)
            {
                Repository = repository;
            }
            public IPersonRepository Repository { get; }
            public Person GetPerson(int personId)
            {
                // get the data from Web service and return Person instance.
                return new Person(this);
            }
        }
    }
}
