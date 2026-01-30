import api from './api'

export async function getTransactionsForEdit(date) {
  return (await api.get(`/transactions/edit?date=${date}`)).data
}

export async function bulkUpsertTransactions(date, items) {
  return (await api.post('/transactions/bulk-upsert', { date, items })).data
}

export async function getTransactionHistory(payload) {
  return (await api.post('/transactions/history', payload)).data
}

export async function getAssetsLookup() {
  return (await api.get('/transactions/lookup')).data
}

/* =========================
   IMPORT
   ========================= */

export async function previewTransactionsImport(file) {
  const form = new FormData()
  form.append('file', file)

  return (await api.post('/transactions/import/preview', form)).data
}

export async function confirmTransactionsImport(rows) {
  return (await api.post('/transactions/import/confirm', { rows })).data
}
