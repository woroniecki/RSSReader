import moment from 'moment'

export const applyValidationErrors = (formik: any, data: any): void => {
  if (data.responseException && data.responseException.validationErrors) {
    data.responseException.validationErrors.forEach((element: any) => {
      formik.setFieldError(element.name.toLowerCase(), element.reason)
    })
  } else if (data.validationErrors) {
    data.validationErrors.forEach((element: any) => {
      formik.setFieldError(element.name.toLowerCase(), element.reason)
    })
  } else if (data.responseException && data.responseException.error) {
    formik.setFieldError('global', data.responseException.error)
  } else if (data.message) {
    formik.setFieldError('global', data.message)
  }
}

export const formatDate = (date: string): string => {
  return moment(date).format('DD/MM/YYYY')
}
