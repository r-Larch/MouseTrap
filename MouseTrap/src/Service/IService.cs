namespace MouseTrap.Service {
    public interface IService {
        void Run(CancellationToken token);
        void OnStart();
        void OnExit();
    }
}
