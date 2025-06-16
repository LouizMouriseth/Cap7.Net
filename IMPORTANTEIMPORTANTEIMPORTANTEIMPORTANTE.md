## Observações importantes
- O docker compose tem um container SQL Server
- A aplicação pode ser testada localmente se o SQL Server estiver instalado
    - A instância fora do container é SQLEXPRESS e usa integrated security para facilitar
    - Uma string diferente é usada no docker e é trocada automaticamente
- O projeto usa vertical slice nos services que estão em application, as viewmodels se encontram dentro deles
- As migrações são feitas automaticamente
- Por via das dúvidas, a estrutura do repositório foi criada, não copiada de algum lugar, segue o link da estrutura
https://github.com/LucasSimionatoIsTaken/Csharp-DDD-Boilerplate
- Com exceção dos endpoints de user/auth, todos exigem autenticação
- O swagger suporta token, se quiser corrigir por ele, coloca só o token que o "Bearer " ele coloca sozinho pelo swagger