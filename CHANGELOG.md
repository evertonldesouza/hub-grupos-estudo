# Changelog

Todas as mudanças relevantes do projeto são registradas neste arquivo. O formato segue a ideia do [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e as versões seguem [Semantic Versioning](https://semver.org/lang/pt-BR/).

## [Unreleased]

Use esta seção durante o desenvolvimento. Mova os itens para uma versão no momento da release.

### Added

- notificações persistidas para propostas e mudanças de estado de trocas;
- notificações de inscrição, progresso e conquistas;
- API autenticada para listar, paginar e marcar notificações como lidas;
- central de notificações na interface web.

### Changed

- ações de troca e progresso passam a registrar eventos para o usuário relacionado;

### Fixed

- espaço reservado para correções;

### Security

- notificações são consultadas e alteradas somente pelo usuário proprietário.

## [1.0.0] - 2026-10-29

### Added

- interface web responsiva servida pela aplicação;
- cadastro, login e autenticação JWT;
- perfil editável e habilidades ensinadas;
- trilhas, etapas, inscrições, avanço e conclusão;
- XP, níveis e conquistas ligados aos eventos de estudo;
- criação, aceite, recusa, cancelamento e conclusão de trocas;
- paginação básica de trilhas;
- rate limiting nos endpoints de autenticação;
- documentação de segurança, testes e CI/CD;
- testes unitários, testes HTTP e integração real com PostgreSQL.

### Changed

- migrations e seed inicial preparados para PostgreSQL;
- CI configurado com PostgreSQL 16, build Release, testes e cobertura;
- Swagger restrito ao ambiente de desenvolvimento.

[Unreleased]: https://github.com/evertonldesouza/hub-grupos-estudo/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/evertonldesouza/hub-grupos-estudo/releases/tag/v1.0.0
