import axios from 'api/axios'

export const setAuthHeader = (token: string) => {
  axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
}

export const removeAuthHeader = () => {
  delete axios.defaults.headers.common['Authorization']
}
