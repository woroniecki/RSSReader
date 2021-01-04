export const applyValidationErrors = (formik: any, response: any) => {
  if (response.validationErrors) {
    response.validationErrors.forEach((element: any) => {
      formik.setFieldError(element.name.toLowerCase(), element.reason)
    })
  } else if (response.message) {
    formik.setFieldError('global', response.message)
  }
}
