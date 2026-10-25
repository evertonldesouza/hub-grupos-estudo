const state = {
  token: localStorage.getItem('hub_token'),
  user: null,
  skills: [],
  tracks: [],
  enrollments: [],
  achievements: [],
  myAchievements: [],
  users: [],
  trades: [],
  level: 'all',
  query: ''
};

const $ = (selector) => document.querySelector(selector);
const $$ = (selector) => [...document.querySelectorAll(selector)];

function showToast(message, isError = false) {
  const toast = $('#toast');
  toast.textContent = message;
  toast.classList.toggle('error', isError);
  toast.classList.add('show');
  clearTimeout(showToast.timer);
  showToast.timer = setTimeout(() => toast.classList.remove('show'), 3200);
}

function escapeHtml(value = '') {
  return String(value)
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#039;');
}

function initials(name = '') {
  return name.trim().split(/\s+/).slice(0, 2).map(part => part[0]).join('').toUpperCase() || '?';
}

function formatDate(value) {
  if (!value) return '';
  return new Intl.DateTimeFormat('pt-BR', { day: '2-digit', month: 'short' }).format(new Date(value));
}

function levelLabel(level) {
  return ({ Iniciante: 'Iniciante', Intermediario: 'Intermediário', Avancado: 'Avançado' })[level] || level;
}

function statusLabel(status) {
  return ({ Pendente: 'Pendente', Ativa: 'Ativa', Concluida: 'Concluída', Recusada: 'Recusada', Cancelada: 'Cancelada' })[status] || status;
}

async function api(path, options = {}) {
  const headers = { ...(options.body ? { 'Content-Type': 'application/json' } : {}), ...(options.headers || {}) };
  if (state.token) headers.Authorization = `Bearer ${state.token}`;

  const response = await fetch(path, { ...options, headers });
  const text = await response.text();
  let payload = null;
  try { payload = text ? JSON.parse(text) : null; } catch { payload = text; }

  if (response.status === 401 && state.token) {
    logout(false);
    throw new Error('Sua sessão expirou. Entre novamente.');
  }

  if (!response.ok) {
    const validation = payload?.errors
      ? Object.values(payload.errors).flat().join(' ')
      : null;
    throw new Error(validation || payload?.mensagem || payload?.title || 'Não foi possível concluir a operação.');
  }

  return payload;
}

function setAuthMode(mode) {
  $$('.tab').forEach(tab => tab.classList.toggle('active', tab.dataset.authMode === mode));
  $('#login-form').classList.toggle('hidden', mode !== 'login');
  $('#register-form').classList.toggle('hidden', mode !== 'register');
}

function showAuth() {
  $('#auth-screen').classList.remove('hidden');
  $('#app-screen').classList.add('hidden');
}

function showApp() {
  $('#auth-screen').classList.add('hidden');
  $('#app-screen').classList.remove('hidden');
}

async function loadApp() {
  showApp();
  try {
    const [user, skills, tracks, enrollments, achievements, myAchievements, users, trades] = await Promise.all([
      api('/api/usuarios/me'),
      api('/api/habilidades'),
      api('/api/trilhas'),
      api('/api/inscricoes'),
      api('/api/conquistas'),
      api('/api/conquistas/minhas'),
      api('/api/usuarios'),
      api('/api/trocas')
    ]);
    Object.assign(state, { user, skills, tracks, enrollments, achievements, myAchievements, users, trades });
    renderAll();
  } catch (error) {
    showToast(error.message, true);
  }
}

function renderAll() {
  renderUserChrome();
  renderDashboard();
  renderTrackForm();
  renderTracks();
  renderUsers();
  renderTrades();
  renderTradeForm();
  renderProfile();
}

function renderUserChrome() {
  const user = state.user || {};
  const name = user.nome || 'Estudante';
  const level = user.nivel || 1;
  const xp = user.xp || 0;
  const progress = xp % 100;
  $('#header-name').textContent = name;
  $('#header-level').textContent = `Nível ${level}`;
  $('#header-avatar').textContent = initials(name);
  $('#sidebar-level').textContent = `Nível ${level}`;
  $('#sidebar-xp').textContent = `${xp} XP`;
  $('#sidebar-xp-bar').style.width = `${progress}%`;
}

function renderDashboard() {
  const active = state.enrollments.filter(item => !item.concluida);
  const completed = state.enrollments.filter(item => item.concluida);
  $('#welcome-name').textContent = (state.user?.nome || 'estudante').split(' ')[0];
  $('#stat-active').textContent = active.length;
  $('#stat-completed').textContent = completed.length;
  $('#stat-achievements').textContent = state.myAchievements.length;
  $('#stat-xp').textContent = state.user?.xp || 0;

  const enrollmentContainer = $('#dashboard-enrollments');
  if (!state.enrollments.length) {
    enrollmentContainer.innerHTML = '<div class="empty-state">Você ainda não começou nenhuma trilha.<br>Que tal explorar um caminho?</div>';
  } else {
    enrollmentContainer.innerHTML = state.enrollments.slice(0, 4).map(enrollment => `
      <article class="enrollment-card">
        <div class="enrollment-icon">${enrollment.concluida ? '✓' : '◒'}</div>
        <div><h3>${escapeHtml(enrollment.trilhaTitulo)}</h3><small>${enrollment.concluida ? 'Concluída' : `Etapa ${enrollment.etapaAtual}`}</small></div>
        <div class="enrollment-right"><strong>${Math.round(enrollment.progressoPct)}%</strong><div class="mini-bar"><i style="width:${enrollment.progressoPct}%"></i></div></div>
      </article>`).join('');
  }

  const achievements = $('#dashboard-achievements');
  achievements.innerHTML = state.myAchievements.length
    ? state.myAchievements.slice(0, 3).map(renderAchievement).join('')
    : '<div class="empty-state">Suas primeiras conquistas aparecem aqui.</div>';
}

function renderAchievement(item) {
  return `<article class="achievement-card"><div class="achievement-icon">✦</div><div><h3>${escapeHtml(item.nome)}</h3><small>${escapeHtml(item.descricao || '')}</small></div></article>`;
}

function renderTrackForm() {
  $('#track-skill-select').innerHTML = state.skills.length
    ? state.skills.map(skill => `<option value="${skill.id}">${escapeHtml(skill.nome)} · ${escapeHtml(skill.categoria)}</option>`).join('')
    : '<option value="">Nenhuma habilidade cadastrada</option>';
}

function filteredTracks() {
  return state.tracks.filter(track => {
    const matchesLevel = state.level === 'all' || track.nivelDificuldade === state.level;
    const query = state.query.toLowerCase();
    return matchesLevel && (!query || `${track.titulo} ${track.descricao} ${track.habilidadeNome}`.toLowerCase().includes(query));
  });
}

function renderTracks() {
  const tracks = filteredTracks();
  const container = $('#tracks-list');
  if (!tracks.length) {
    container.innerHTML = '<div class="empty-state">Nenhuma trilha encontrada com esses filtros.</div>';
    return;
  }

  container.innerHTML = tracks.map(track => {
    const enrollment = state.enrollments.find(item => item.trilhaId === track.id);
    const action = enrollment
      ? (enrollment.concluida
        ? '<button class="button secondary" disabled>Concluída ✓</button>'
        : `<button class="button primary" data-track-action="advance" data-enrollment-id="${enrollment.id}">Avançar →</button>`)
      : `<button class="button secondary" data-track-action="enroll" data-track-id="${track.id}">Começar</button>`;
    const progress = enrollment ? `<div class="mini-bar" title="${Math.round(enrollment.progressoPct)}% concluído"><i style="width:${enrollment.progressoPct}%"></i></div>` : '';
    return `<article class="track-card">
      <div class="track-top"><span class="category-tag">${escapeHtml(track.habilidadeNome || 'Estudos')}</span><span class="level-tag">${levelLabel(track.nivelDificuldade)}</span></div>
      <h3>${escapeHtml(track.titulo)}</h3><p>${escapeHtml(track.descricao)}</p>
      <div class="track-footer"><span class="track-meta">${track.totalEtapas} ${track.totalEtapas === 1 ? 'etapa' : 'etapas'} · por ${escapeHtml(track.criadorNome || 'comunidade')}</span><div>${progress}${action}</div></div>
    </article>`;
  }).join('');
}

function renderUsers() {
  const query = ($('#user-search')?.value || '').toLowerCase();
  const users = state.users.filter(user => `${user.nome} ${user.bio || ''}`.toLowerCase().includes(query));
  const container = $('#users-list');
  if (!users.length) {
    container.innerHTML = '<div class="empty-state">Nenhuma pessoa encontrada.</div>';
    return;
  }
  container.innerHTML = users.map(user => `<article class="user-card"><div class="avatar">${initials(user.nome)}</div><div class="user-card-copy"><h3>${escapeHtml(user.nome)}${user.id === state.user?.id ? ' <small>(você)</small>' : ''}</h3><p>${escapeHtml(user.bio || `Nível ${user.nivel} · ${user.xp} XP`)}</p><div class="user-skills">${(user.habilidades || []).slice(0, 4).map(skill => `<span>${escapeHtml(skill.nome)}</span>`).join('') || '<span>Aprendendo</span>'}</div></div></article>`).join('');
}

function renderTrades() {
  const container = $('#trades-list');
  if (!state.trades.length) {
    container.innerHTML = '<div class="empty-state">Você ainda não tem trocas. Explore a comunidade para encontrar alguém.</div>';
    return;
  }
  container.innerHTML = state.trades.map(trade => {
    const isB = trade.usuarioBId === state.user?.id;
    const actions = [];
    if (trade.status === 'Pendente' && isB) {
      actions.push(`<button data-trade-action="accept" data-trade-id="${trade.id}">Aceitar</button>`);
      actions.push(`<button class="danger" data-trade-action="reject" data-trade-id="${trade.id}">Recusar</button>`);
    }
    if (['Pendente', 'Ativa'].includes(trade.status)) {
      actions.push(`<button class="danger" data-trade-action="cancel" data-trade-id="${trade.id}">Cancelar</button>`);
    }
    if (trade.status === 'Ativa') actions.push(`<button data-trade-action="complete" data-trade-id="${trade.id}">Concluir</button>`);
    return `<article class="trade-card"><div><h3>${escapeHtml(trade.usuarioANome)} ↔ ${escapeHtml(trade.usuarioBNome)}</h3><p>Troca de conhecimento · ${formatDate(trade.criadoEm)}</p><span class="trade-status ${trade.status}">${statusLabel(trade.status)}</span></div><div class="trade-actions">${actions.join('')}</div></article>`;
  }).join('');
}

function renderTradeForm() {
  const userSelect = $('#trade-user-select');
  const myTrackSelect = $('#trade-my-track');
  const theirTrackSelect = $('#trade-their-track');
  if (!userSelect || !myTrackSelect || !theirTrackSelect) return;

  const otherUsers = state.users.filter(user => user.id !== state.user?.id);
  userSelect.innerHTML = otherUsers.length
    ? otherUsers.map(user => `<option value="${user.id}">${escapeHtml(user.nome)}</option>`).join('')
    : '<option value="">Nenhuma outra pessoa disponível</option>';

  const myTracks = state.tracks.filter(track => track.criadorId === state.user?.id);
  myTrackSelect.innerHTML = myTracks.length
    ? myTracks.map(track => `<option value="${track.id}">${escapeHtml(track.titulo)}</option>`).join('')
    : '<option value="">Crie uma trilha primeiro</option>';

  updateTradeTrackOptions();
}

function updateTradeTrackOptions() {
  const userId = $('#trade-user-select')?.value;
  const theirTrackSelect = $('#trade-their-track');
  const myTrackSelect = $('#trade-my-track');
  const mySkillSelect = $('#trade-my-skill');
  const theirSkillSelect = $('#trade-their-skill');
  if (!theirTrackSelect || !myTrackSelect || !mySkillSelect || !theirSkillSelect) return;

  const theirTracks = state.tracks.filter(track => track.criadorId === userId);
  theirTrackSelect.innerHTML = theirTracks.length
    ? theirTracks.map(track => `<option value="${track.id}">${escapeHtml(track.titulo)}</option>`).join('')
    : '<option value="">Essa pessoa ainda não criou trilhas</option>';

  const myTrack = state.tracks.find(track => track.id === myTrackSelect.value);
  const theirTrack = state.tracks.find(track => track.id === theirTrackSelect.value);
  mySkillSelect.innerHTML = myTrack
    ? `<option value="${myTrack.habilidadeId}">${escapeHtml(myTrack.habilidadeNome)}</option>`
    : '<option value="">Escolha uma trilha sua</option>';
  theirSkillSelect.innerHTML = theirTrack
    ? `<option value="${theirTrack.habilidadeId}">${escapeHtml(theirTrack.habilidadeNome)}</option>`
    : '<option value="">Escolha uma trilha da pessoa</option>';
}

function renderProfile() {
  const user = state.user || {};
  const name = user.nome || 'Estudante';
  $('#profile-avatar').textContent = initials(name);
  $('#profile-name').textContent = name;
  $('#profile-meta').textContent = `Nível ${user.nivel || 1} · ${user.xp || 0} XP`;
  $('#profile-name-input').value = name;
  $('#profile-bio-input').value = user.bio || '';

  const selected = new Set((user.habilidades || []).map(skill => skill.id));
  $('#skills-list').innerHTML = state.skills.map(skill => `<label class="skill-check"><input type="checkbox" value="${skill.id}" ${selected.has(skill.id) ? 'checked' : ''}> <span>${escapeHtml(skill.nome)}</span></label>`).join('');
  $('#profile-achievements').innerHTML = state.myAchievements.length
    ? state.myAchievements.map(renderAchievement).join('')
    : '<div class="empty-state">Ainda não há conquistas desbloqueadas.</div>';
}

async function refreshData() {
  const [user, tracks, enrollments, myAchievements, users, trades] = await Promise.all([
    api('/api/usuarios/me'), api('/api/trilhas'), api('/api/inscricoes'), api('/api/conquistas/minhas'), api('/api/usuarios'), api('/api/trocas')
  ]);
  Object.assign(state, { user, tracks, enrollments, myAchievements, users, trades });
  renderAll();
}

async function handleLogin(event) {
  event.preventDefault();
  const data = Object.fromEntries(new FormData(event.currentTarget));
  try {
    const result = await api('/api/auth/login', { method: 'POST', body: JSON.stringify(data) });
    state.token = result.token;
    localStorage.setItem('hub_token', state.token);
    await loadApp();
    showToast('Bem-vindo de volta!');
  } catch (error) { showToast(error.message, true); }
}

async function handleRegister(event) {
  event.preventDefault();
  const data = Object.fromEntries(new FormData(event.currentTarget));
  try {
    await api('/api/auth/registrar', { method: 'POST', body: JSON.stringify(data) });
    $('#login-form [name="email"]').value = data.email;
    setAuthMode('login');
    showToast('Conta criada. Agora entre para começar.');
  } catch (error) { showToast(error.message, true); }
}

function logout(notify = true) {
  state.token = null;
  localStorage.removeItem('hub_token');
  showAuth();
  if (notify) showToast('Você saiu do hub.');
}

async function handleCreateTrack(event) {
  event.preventDefault();
  const data = Object.fromEntries(new FormData(event.currentTarget));
  const body = {
    trilha: { titulo: data.titulo, descricao: data.descricao, nivelDificuldade: data.nivelDificuldade, habilidadeId: data.habilidadeId },
    etapas: [{ titulo: data.etapaTitulo, descricao: data.etapaDescricao, tipo: 'Artigo', duracaoEstimadaMin: Number(data.duracao) }]
  };
  try {
    await api('/api/trilhas', { method: 'POST', body: JSON.stringify(body) });
    event.currentTarget.reset();
    $('#create-track-panel').classList.add('hidden');
    await refreshData();
    showToast('Trilha publicada com sucesso!');
  } catch (error) { showToast(error.message, true); }
}

async function handleTrackAction(button) {
  try {
    if (button.dataset.trackAction === 'enroll') {
      await api('/api/inscricoes', { method: 'POST', body: JSON.stringify({ trilhaId: button.dataset.trackId }) });
      showToast('Inscrição realizada. Bons estudos!');
    } else {
      await api(`/api/inscricoes/${button.dataset.enrollmentId}/avancar`, { method: 'POST' });
      showToast('Etapa avançada! Você está evoluindo.');
    }
    await refreshData();
  } catch (error) { showToast(error.message, true); }
}

async function handleTradeAction(button) {
  const actionMap = { accept: 'ativar', reject: 'recusar', cancel: 'cancelar', complete: 'concluir' };
  try {
    await api(`/api/trocas/${button.dataset.tradeId}/${actionMap[button.dataset.tradeAction]}`, { method: 'POST' });
    await refreshData();
    showToast('Troca atualizada.');
  } catch (error) { showToast(error.message, true); }
}

async function handleCreateTrade(event) {
  event.preventDefault();
  const data = Object.fromEntries(new FormData(event.currentTarget));
  const body = {
    usuarioBId: data.usuarioBId,
    habilidadeAId: data.habilidadeAId,
    habilidadeBId: data.habilidadeBId,
    trilhaAId: data.trilhaAId,
    trilhaBId: data.trilhaBId
  };

  try {
    await api('/api/trocas', { method: 'POST', body: JSON.stringify(body) });
    await refreshData();
    showToast('Proposta enviada para a comunidade.');
  } catch (error) { showToast(error.message, true); }
}

async function handleProfile(event) {
  event.preventDefault();
  const data = Object.fromEntries(new FormData(event.currentTarget));
  try {
    await api('/api/usuarios/me', { method: 'PUT', body: JSON.stringify(data) });
    await refreshData();
    showToast('Perfil atualizado.');
  } catch (error) { showToast(error.message, true); }
}

async function handleSkills() {
  const habilidadeIds = $$('#skills-list input:checked').map(input => input.value);
  try {
    await api('/api/usuarios/me/habilidades', { method: 'PUT', body: JSON.stringify({ habilidadeIds }) });
    await refreshData();
    showToast('Habilidades atualizadas.');
  } catch (error) { showToast(error.message, true); }
}

function showSection(section) {
  $$('.page-section').forEach(item => item.classList.toggle('active-section', item.id === `section-${section}`));
  $$('.nav-item').forEach(item => item.classList.toggle('active', item.dataset.section === section));
  window.scrollTo({ top: 0, behavior: 'smooth' });
}

function bindEvents() {
  $$('.tab').forEach(tab => tab.addEventListener('click', () => setAuthMode(tab.dataset.authMode)));
  $('#login-form').addEventListener('submit', handleLogin);
  $('#register-form').addEventListener('submit', handleRegister);
  $('#logout-button').addEventListener('click', () => logout());
  $('#create-track-form').addEventListener('submit', handleCreateTrack);
  $('#trade-form').addEventListener('submit', handleCreateTrade);
  $('#trade-user-select').addEventListener('change', updateTradeTrackOptions);
  $('#trade-my-track').addEventListener('change', updateTradeTrackOptions);
  $('#trade-their-track').addEventListener('change', updateTradeTrackOptions);
  $('#toggle-create-track').addEventListener('click', () => $('#create-track-panel').classList.toggle('hidden'));
  $('#close-create-track').addEventListener('click', () => $('#create-track-panel').classList.add('hidden'));
  $('#profile-form').addEventListener('submit', handleProfile);
  $('#save-skills').addEventListener('click', handleSkills);
  $('#track-search').addEventListener('input', event => { state.query = event.target.value; renderTracks(); });
  $('#user-search').addEventListener('input', renderUsers);
  $('#track-filters').addEventListener('click', event => {
    const pill = event.target.closest('[data-level]');
    if (!pill) return;
    state.level = pill.dataset.level;
    $$('#track-filters .pill').forEach(item => item.classList.toggle('active', item === pill));
    renderTracks();
  });
  document.addEventListener('click', event => {
    const sectionButton = event.target.closest('[data-section]');
    if (sectionButton) { event.preventDefault(); showSection(sectionButton.dataset.section); }
    const trackButton = event.target.closest('[data-track-action]');
    if (trackButton) handleTrackAction(trackButton);
    const tradeButton = event.target.closest('[data-trade-action]');
    if (tradeButton) handleTradeAction(tradeButton);
  });
}

bindEvents();
if (state.token) loadApp(); else showAuth();
