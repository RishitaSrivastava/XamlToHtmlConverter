class BindingResolver {

    static resolve(obj, path) {
        if (!path) return undefined

        const parts = path.split(".")
        let value = obj

        for (const p of parts) {
            if (value == null) return undefined
            value = value[p]
        }

        return value
    }
}

class BindingEngine {

    static apply(root, viewModel) {

        // text binding
        root.querySelectorAll("[data-binding-text]").forEach(el => {

            const path = el.dataset.bindingText
            const value = BindingResolver.resolve(viewModel, path)

            if (value !== undefined)
                el.textContent = value
        })

        // input value binding
        root.querySelectorAll("[data-binding-value]").forEach(el => {

            const path = el.dataset.bindingValue
            const value = BindingResolver.resolve(viewModel, path)

            if (value !== undefined)
                el.value = value
        })

        // checkbox binding
        root.querySelectorAll("[data-binding-checked]").forEach(el => {

            const path = el.dataset.bindingChecked
            const value = BindingResolver.resolve(viewModel, path)

            el.checked = !!value
        })
    }
}

class CommandEngine {

    static apply(root, viewModel) {

        root.querySelectorAll("[data-binding-command]").forEach(el => {

            const cmd = el.dataset.bindingCommand

            el.addEventListener("click", () => {

                const fn = viewModel[cmd]

                if (typeof fn === "function")
                    fn.call(viewModel)
            })
        })
    }
}

class EventEngine {

    static apply(root, controller) {

        root.querySelectorAll("[data-event-click]").forEach(el => {

            const handler = el.dataset.eventClick

            el.addEventListener("click", () => {

                const fn = controller[handler]

                if (typeof fn === "function")
                    fn.call(controller)
            })
        })
    }
}

class ItemsSourceEngine {

    static apply(root, viewModel) {

        root.querySelectorAll("[data-binding-itemssource]").forEach(list => {

            const source = list.dataset.bindingItemssource
            const items = BindingResolver.resolve(viewModel, source)

            if (!Array.isArray(items))
                return

            const template = list.querySelector("option")

            if (!template)
                return

            const templateHTML = template.outerHTML

            list.innerHTML = ""

            items.forEach(item => {

                const wrapper = document.createElement("div")
                wrapper.innerHTML = templateHTML

                const node = wrapper.firstElementChild

                BindingEngine.apply(node, item)

                list.appendChild(node)
            })
        })
    }
}

class XamlRuntime {

    static start(viewModel, controller) {

        const root = document.body

        ItemsSourceEngine.apply(root, viewModel)
        BindingEngine.apply(root, viewModel)
        CommandEngine.apply(root, viewModel)
        EventEngine.apply(root, controller)
    }
}